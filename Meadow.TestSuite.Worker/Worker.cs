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

        public ITestRegistry Registry { get; }
        internal ITestProvider Provider { get; }
        public ICommandSerializer Serializer { get; }
        public ITestListener Listener { get; }
        public IResultsStore Results => m_resultsStore;

        public Worker(F7Micro device)
        {
            // TODO: make this all configurable
            m_resultsStore = new ResultsStore();
            Registry = Provider = new WorkerRegistry("/meadow0/test", device);

            Console.WriteLine(" Creating serial port...");
            var port = device.CreateSerialPort(
                device.SerialPortNames.Com4,
                9600,
                8,
                Parity.None,
                StopBits.One);

            Serializer = new CommandJsonSerializer(JsonLibrary.SystemTextJson);

            Listener = new MeadowSerialListener(port, Serializer);
            Listener.CommandReceived += Listener_CommandReceived;
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
            // TODO: make this non-blocking?
            Listener.StartListening();
        }
    }
}