using Meadow.Devices;
using Meadow.Hardware;
using Meadow.TestSuite;
using Meadow.Units;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("TestSuite.Unit.Tests")]

namespace MeadowApp
{
    public class Worker : IWorker
    {
        private ResultsStore m_resultsStore;
        private IPin m_green = null;
        private IPin m_red = null;

        public ITestRegistry Registry { get; private set; }
        internal ITestProvider Provider { get; private set; }
        public ICommandSerializer Serializer { get; private set; }
        public ITestListener Listener { get; private set; }
        public ITestDisplay? Display { get; private set; }

        public IResultsStore Results => m_resultsStore;
        private F7Micro Device { get; }
        private Config? Config { get; set; } = null;

        public Worker(F7Micro device)
        {
            m_resultsStore = new ResultsStore();
            Registry = Provider = new WorkerRegistry("/meadow0/test", Device);
            Serializer = new CommandJsonSerializer(JsonLibrary.SystemTextJson);

            // TODO: handle v2 device
            Device = device;
        }

        public void Configure(Config config)
        {
            Console.WriteLine($" Configuring Worker...");

            Config = config;

            if (!string.IsNullOrEmpty(config.Display))
            {
                switch (config.Display.ToUpper())
                {
                    case "ST7789":
                        Display = new ST7789TestDisplay(Device);
                        Display.ShowText(0, "TestSuite"); // TODO: show version
                        break;
                    default:
                        Console.WriteLine($"Unsupported Display Requested: {config.Display}");
                        break;
                }
            }

            // TODO: make this all configurable
            if (config.Network != null)
            {
                try
                {
                    Console.WriteLine($"Initializing WiFi...");
                    Device.InitWiFiAdapter();

                    // TODO: only do this if we're not connected
                    Device.WiFiAdapter.WiFiConnected += WiFiAdapter_WiFiConnected;

                    Console.WriteLine($" Connecting to WiFi ssid {config.Network.SSID}...");
                    Display.ShowText(1, $"--> {config.Network.SSID}");
                    Device.WiFiAdapter.Connect(config.Network.SSID, config.Network.Pass);

                    while (!Device.WiFiAdapter.IsConnected)
                    {
                        Console.WriteLine("Waiting to connect to AP....");
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine($"Local IP: {Device.WiFiAdapter.IpAddress}");
                    Display.ShowText(1, $"IP: {Device.WiFiAdapter.IpAddress}");

                    Listener = new MeadowNetworkListener(Device.WiFiAdapter.IpAddress, config, Serializer);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
            }
            else
            {
                // serial fallback

                Console.WriteLine(" Creating serial port...");
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
            Console.WriteLine(" WiFi connected");            
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
                        break;
                    case TestState.Failed:
                        red.State = true;
                        break;
                    default:
                        green.State = true;
                        red.State = true;
                        break;
                }
            }
        }

        public TestResult ExecuteTest(string testID)
        {
            if(Config == null)
            {
                Console.WriteLine($" Failure: Worker not configured");
                return null;
            }

            try
            {
                var runner = new TestRunner(Provider, testID);
                var result = runner.Begin();

                // store the result
                m_resultsStore.Add(result);

                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine($" Failure: {ex.Message}");
                return null;
            }
        }

        private void Listener_CommandReceived(ITestListener listener, TestCommand command)
        {
            if (command == null)
            {
                Console.WriteLine(" ** NULL COMMAND** ");
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
                Console.WriteLine($" Failure: Worker not configured");
                return;
            }

            // wait for a listener (wifi is async)
            while(Listener == null)
            {
                Console.WriteLine($"{Device.WiFiAdapter.IpAddress}");

                Thread.Sleep(1000);
            }

            // TODO: make this non-blocking?
            Listener.StartListening();
        }
    }
}