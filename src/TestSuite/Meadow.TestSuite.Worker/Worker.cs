using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.TestSuite;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TestSuite.Unit.Tests")]

namespace MeadowApp
{
    public class Worker : IWorker
    {
        private IPin m_green = null;
        private IPin m_red = null;

        public ICommandSerializer Serializer { get; private set; }
        public ITestListener Listener { get; private set; }
        public ITestDisplay? Display { get; private set; }
        public Logger Logger { get; }
        public IResultsStore Results { get; }
        public ITestRegistry Registry { get; private set; }
        internal ITestProvider Provider { get; private set; }
        public Config? Config { get; private set; }

        public bool EnableDebugging { get; set; } = false;

        private F7FeatherV2 Device { get; }

        public Worker(F7FeatherV2 device)
        {
            Logger = new Logger(new ConsoleLogProvider());
            //Logger.AddProvider(new ConsoleLogProvider());
            Results = new ResultsStore();

            Serializer = new CommandJsonSerializer(JsonLibrary.SimpleJson);

            // TODO: handle v2 device
            Device = device;
        }

        public async Task Configure(Config config)
        {
            if (config == null)
            {
                Logger.Warn($"Configuration is null");
                config = Config.Default;
            }

            Logger.Info($" Configuring Worker...");

            Config = config;

            Logger.Info($" Test assemblies are at {Config.TestAssemblyFolder}");

            var di = new DirectoryInfo(Config.TestAssemblyFolder);
            if (!di.Exists)
            {
                di.Create();
            }
            else
            {
                var files = di.GetFiles();
                if (files == null || files.Length == 0)
                {
                    Logger.Warn("No existing files in Test folder.");
                }
                else
                {
                    Logger.Info("Existing files in Test folder:");
                    foreach (var f in di.GetFiles())
                    {
                        Console.WriteLine($"  {f}");
                    }
                }
            }
            Registry = Provider = new WorkerRegistry(Config.TestAssemblyFolder, Device);

            if (!string.IsNullOrEmpty(config.Display))
            {
                switch (config.Display.ToUpper())
                {
                    case "ST7789":
                        Logger.Info("Configured for ST7789 Display");
                        Display = new ST7789TestDisplay(Device);
                        break;
                    default:
                        Logger.Error($"Unsupported Display Requested: {config.Display}");
                        break;
                }
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Display?.ShowText(0, $"TestSuite v{version.ToString(3)}");

            // TODO: make this all configurable
            if (config.Network != null)
            {
                try
                {
                    var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

                    Logger.Info($"Initializing WiFi...");

                    // TODO: only do this if we're not connected
                    wifi.NetworkConnected += (s, e) =>
                    {
                        Logger.Info(" WiFi connected");
                    };

                    Logger.Info($" Connecting to WiFi ssid {config.Network.SSID}...");
                    Display?.ShowText(1, $"--> {config.Network.SSID}");
                    await wifi.Connect(config.Network.SSID, config.Network.Pass);

                    /*
                    while (!Device.WiFiAdapter.IsConnected)
                    {
                        Logger.Info($"Waiting to connect to AP.... {Device.WiFiAdapter.IpAddress}");
                        Thread.Sleep(1000);

                        // work-around for a 0.6.0.8 bug
                        if (!Device.WiFiAdapter.IpAddress.Equals(IPAddress.Any))
                        {
                            Logger.Warn("IP Address received while IsConnected is still False");
                            break;
                        }
                    }
                    */

                    Logger.Info($"Local IP: {wifi.IpAddress}");
                    Display?.ShowText(1, $"IP: {wifi.IpAddress}");
                    Display?.ShowText(2, $"Port: 8080"); // TODO: make port configurable (it's hard coded in the listener below)

                    Listener = new MeadowNetworkListener(wifi.IpAddress, config, Serializer, Logger);
                }
                catch (Exception ex)
                {
                    Logger.Error($"{ex.Message}");
                }
            }
            else
            {
                // serial fallback

                Logger.Info(" Creating serial port...");
                var port = Device.CreateSerialPort(
                    Device.SerialPortNames.Com4,
                    9600,
                    8,
                    Parity.None,
                    StopBits.One);

                Listener = new MeadowSerialListener(port, Serializer, Logger);
                Listener.CommandReceived += Listener_CommandReceived;
            }
        }

        private void WiFiAdapter_WiFiConnected(object sender, EventArgs e)
        {
            Logger.Info(" WiFi connected");
        }

        public void IndicateState(TestState state)
        {
            if (m_green == null)
            {
                m_green = Provider.Device.GetPin("OnboardLedGreen");
            }
            if (m_red == null)
            {
                m_red = Provider.Device.GetPin("OnboardLedRed");
            }


            using (var red = Provider.Device.CreateDigitalOutputPort(m_red, false))
            using (var green = Provider.Device.CreateDigitalOutputPort(m_green, false))
            {
                switch (state)
                {
                    case TestState.Success:
                        green.State = true;
                        Display?.ShowText(5, "PASS");
                        break;
                    case TestState.Failed:
                        red.State = true;
                        Display?.ShowText(5, "FAIL");
                        break;
                    default:
                        green.State = true;
                        red.State = true;
                        Display?.ShowText(5, "");
                        break;
                }
            }
        }

        public TestResult ExecuteTest(string testID)
        {
            if (Config == null)
            {
                Logger.Error($" Failure: Worker not configured");
                return null;
            }

            try
            {
                var runner = new TestRunner(Provider, testID, Logger);
                runner.ShowDebug = EnableDebugging;

                Display?.ShowText(4, "Executing");
                var result = runner.Begin();

                runner.ExecutionComplete += (s, e) =>
                {
                    Display?.ShowText(4, runner.Result.State.ToString());
                };

                // store the result
                Results.Add(result);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($" Failure: {ex.Message}");
                return null;
            }
        }

        private void Listener_CommandReceived(ITestListener listener, TestCommand command)
        {
            if (command == null)
            {
                Logger.Warn(" ** NULL COMMAND** ");
            }
            else
            {
                command.BeforeExecute();
                command.Execute(this);
                command.AfterExecute();


                listener.SendResult(command.Result);
            }
        }

        public void Start()
        {
            if (Config == null)
            {
                Logger.Error($" Failure: Worker not configured");
                return;
            }

            // wait for a listener (wifi is async)
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            while (Listener == null)
            {
                Logger.Info($"{wifi.IpAddress}");

                Thread.Sleep(1000);
            }

            // TODO: make this non-blocking?
            Listener.StartListening();

            Console.WriteLine("Worker Started");
        }
    }
}