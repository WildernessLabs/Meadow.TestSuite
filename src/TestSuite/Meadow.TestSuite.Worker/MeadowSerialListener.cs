﻿using Meadow.Hardware;
using Meadow.Logging;
using Meadow.TestSuite;
using System;
using System.Threading;

namespace MeadowApp
{
    public class MeadowSerialListener : List
    {
        private ISerialPort Port { get; }

        public static TimeSpan WriteTimeout = TimeSpan.FromSeconds(5);

        public MeadowSerialListener(ISerialPort port, ICommandSerializer serializer, Logger logger)
            : base(serializer)
        {
            Port = port;
        }

        public override void StartListening()
        {
            Console.WriteLine(" Opening serial port...");
            try
            {
                Port.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to open serial port: {ex.Message}");
                return;
            }

            Thread.Sleep(100); // give enough time to pull any driver data from the UART before clearing it
            Port.ClearReceiveBuffer();
            Port.DataReceived += Port_DataReceived;
            Port.BufferOverrun += (s, e) =>
            {
                Console.WriteLine($" Port buffer overrun");
            };
            
            Port.WriteTimeout = WriteTimeout;

            Console.WriteLine(" Listening for serial commands...");
            while (true)
            {
                Thread.Sleep(1000);

                if (Port.BytesToRead > 0)
                {
                    Console.WriteLine($"No event {Port.BytesToRead} bytes");

                    var b = new byte[Port.BytesToRead];
                    Port.Read(b, 0, b.Length);
                    HandleData(b, 0, b.Length);
                }
            }
        }

        public override void SendResult(object result)
        {
            if(result == null)
            {
                result = "ok.";
            }

            Console.WriteLine($"Result is {result}. Serializing...");

            var buffer = Serializer.SerializeResult(result);

            // send length
            Console.WriteLine($"Sending length of {buffer.Length}");
            var lengthBytes = BitConverter.GetBytes(buffer.Length);
            Console.WriteLine(BitConverter.ToString(lengthBytes));
            
            Port.Write(lengthBytes);

            Thread.Sleep(500); // TEST CODE

            // send result
            // TODO: add in a timeout here
            Console.WriteLine($"Sending {buffer.Length} bytes");
            Port.Write(buffer);
        }
        
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Port.BytesToRead <= 0) return;

            var b = new byte[Port.BytesToRead];
            Port.Read(b, 0, b.Length);
            HandleData(b, 0, b.Length);
        }
    }
}