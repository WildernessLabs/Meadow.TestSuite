using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Meadow.TestSuite
{
    /// <summary>
    /// A class intended for tests only
    /// </summary>
    public class DesktopSerialListener : List
    {
        private SerialPort Port { get; }

        public DesktopSerialListener(SerialPort port, ICommandSerializer serializer)
            : base(serializer)
        {
            Port = port;
        }

        public override void StartListening()
        {
            Debug.WriteLine(" Opening serial port...");
            try
            {
                if (!Port.IsOpen)
                {
                    Port.Open();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" Failed to open serial port: {ex.Message}");
                return;
            }

            Thread.Sleep(100); // give enough time to pull any driver data from the UART before clearing it
            Port.DiscardInBuffer();
            Port.DataReceived += Port_DataReceived;

            Debug.WriteLine(" Listening for serial commands...");
            while (true)
            {
                Thread.Sleep(1000);

                Debug.WriteLine(".");

                if (!Port.IsOpen)
                {
                    Console.WriteLine($"Serial port closed");
                    break;
                }

                if (Port.BytesToRead > 0)
                {
                    Console.WriteLine($"No event {Port.BytesToRead} bytes");

                    var b = new byte[Port.BytesToRead];
                    Port.Read(b, 0, b.Length);
                    HandleData(b, 0, b.Length);
                }
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Port.BytesToRead <= 0) return;

            var b = new byte[Port.BytesToRead];
            Port.Read(b, 0, b.Length);
            HandleData(b, 0, b.Length);
        }

        public override void SendResult(object result)
        {
            throw new NotImplementedException();
        }
    }
}