using Meadow.Devices;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Hardware;
using Meadow.TestSuite;
using Meadow.Units;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("TestSuite.Unit.Tests")]

namespace MeadowApp
{
    internal static class AppState
    {
        public static Config? Config { get; set; } = null;
        public static ILogger Logger { get; set; }
        public static ITestRegistry Registry { get; set; }
        public static ITestProvider Provider { get; set; }
        public static IResultsStore ResultsStore { get; set; }
    }

    public class Worker : IWorker
    {
        private ResultsStore m_resultsStore;
        private IPin m_green = null;
        private IPin m_red = null;

        public ICommandSerializer Serializer { get; private set; }
        public ITestListener Listener { get; private set; }
        public ITestDisplay? Display { get; private set; }

        private F7Micro Device { get; }

        public Worker(F7Micro device)
        {
            Logger = new ConsoleLogger();
            Logger.Loglevel = Loglevel.Info;
            Results = new ResultsStore();

            Serializer = new CommandJsonSerializer(JsonLibrary.SimpleJson);

            // TODO: handle v2 device
            Device = device;
        }

        public Config? Config
        {
            get => AppState.Config;
            private set => AppState.Config = value;
        }

        public ILogger Logger
        {
            get => AppState.Logger;
            private set => AppState.Logger = value;
        }

        public ITestRegistry Registry
        {
            get => AppState.Registry;
            private set => AppState.Registry = value;
        }

        internal ITestProvider Provider
        {
            get => AppState.Provider;
            private set => AppState.Provider = value;
        }

        public IResultsStore Results
        {
            get => AppState.ResultsStore;
            private set => AppState.ResultsStore = value;
        }

        public void Configure(Config config)
        {
            if(config == null)
            {
                Logger.Warn($"Configuration is null");
                config = Config.Default;
            }

            Logger.Info($" Configuring Worker...");

            Config = config;

            Console.WriteLine($" Test assemblies are at {Config.TestAssemblyFolder}");

            var di = new DirectoryInfo(Config.TestAssemblyFolder);
            if (!di.Exists)
            {
                di.Create();
            }
            else
            {
                Console.WriteLine("Existing files in Test folder:");
                foreach (var f in di.GetFiles())
                {
                    Console.WriteLine($"  {f}");
                }
            }
            Registry = Provider = new WorkerRegistry(Config.TestAssemblyFolder, Device);

            if (!string.IsNullOrEmpty(config.Display))
            {
                switch (config.Display.ToUpper())
                {
                    case "ST7789":
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
                    Logger.Info($"Initializing WiFi...");
                    Device.InitWiFiAdapter();

                    // TODO: only do this if we're not connected
                    Device.WiFiAdapter.WiFiConnected += WiFiAdapter_WiFiConnected;

                    Logger.Info($" Connecting to WiFi ssid {config.Network.SSID}...");
                    Display?.ShowText(1, $"--> {config.Network.SSID}");
                    Device.WiFiAdapter.Connect(config.Network.SSID, config.Network.Pass);

                    while (!Device.WiFiAdapter.IsConnected)
                    {
                        Logger.Info("Waiting to connect to AP....");
                        Thread.Sleep(1000);
                    }

                    Logger.Info($"Local IP: {Device.WiFiAdapter.IpAddress}");
                    Display?.ShowText(1, $"IP: {Device.WiFiAdapter.IpAddress}");

                    Listener = new MeadowNetworkListener(Device.WiFiAdapter.IpAddress, config, Serializer, Logger);
                }
                catch(Exception ex)
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

                Listener = new MeadowSerialListener(port, Serializer);
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
            if(Config == null)
            {
                Logger.Error($" Failure: Worker not configured");
                return null;
            }

            try
            {
                var runner = new TestRunner(Provider, testID);

                Display?.ShowText(4, "Executing");
                var result = runner.Begin();

                // store the result
                m_resultsStore.Add(result);

                return result;
            }
            catch(Exception ex)
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
            while(Listener == null)
            {
                Logger.Info($"{Device.WiFiAdapter.IpAddress}");

                Thread.Sleep(1000);
            }

            // TODO: make this non-blocking?
            Listener.StartListening();
        }
    }
}