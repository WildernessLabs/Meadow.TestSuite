using Meadow.Devices;
using Meadow.TestSuite;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestSuite.Unit.Tests")]

namespace MeadowApp
{
    public class Worker : IWorker
    {
        public F7Micro Device { get; set; }
        public ITestRegistry Registry { get; }
        internal ITestProvider Provider { get; }
        public ICommandSerializer Serializer { get; }
        public ITestListener Listener { get; }

        public Worker(F7Micro device)
        {
            // TODO: make this all configurable
            Registry = Provider = new WorkerRegistry("/meadow0/test");

            Device = device;

            Console.WriteLine(" Creating serial port...");
            var port = Device.CreateSerialPort(
                Device.SerialPortNames.Com4,
                9600,
                8,
                Meadow.Hardware.
                Parity.None,
                Meadow.Hardware.StopBits.One);

            Serializer = new CommandJsonSerializer(JsonLibrary.SystemTextJson);

            Listener = new MeadowSerialListener(port, Serializer);
            Listener.CommandReceived += Listener_CommandReceived;
        }

        public void ExecuteTest(string testID)
        {
            // TODO: record and return some result
            try
            {
                var test = Provider.GetTest(testID);
                if (test == null)
                {
                    Console.WriteLine(" Unknown Test ");
                    return;
                }

                Console.WriteLine($" Creating test instance");
                var instance = test.TestConstructor.Invoke(null);

                // inject Device
                Console.WriteLine($" Checking Device");
                if (test.DeviceProperty != null)
                {
                    Console.WriteLine($" Setting Device");
                    test.DeviceProperty.SetValue(instance, this.Device);
                }

                Console.WriteLine($" Invoking {test?.TestMethod.Name}");
                test.TestMethod.Invoke(instance, null);
            }
            catch(Exception ex)
            {
                Console.WriteLine($" Failure: {ex.Message}");
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