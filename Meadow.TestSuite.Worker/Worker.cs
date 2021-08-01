using Meadow.Devices;
using Meadow.Hardware;
using Meadow.TestSuite;
using System;
using System.Runtime.CompilerServices;

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
            Config = config;

            // TODO: make this all configurable

            if (config.Network != null)
            {
                Console.WriteLine($" Connecting to WiFi ssid {config.Network.SSID}...");
                Device.InitWiFiAdapter();

                // TODO: only do this if we're not connected
                Device.WiFiAdapter.WiFiConnected += WiFiAdapter_WiFiConnected;
                Device.WiFiAdapter.Connect(config.Network.SSID, config.Network.Pass);
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

            // TODO: build up a listener
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

            // TODO: make this non-blocking?
            Listener.StartListening();
        }
    }
}