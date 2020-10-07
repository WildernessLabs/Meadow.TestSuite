﻿using Meadow.Hardware;
using Meadow.TestSuite;
using System;
using System.Text;
using System.Threading;

namespace MeadowApp
{
    public class MeadowSerialListener : ByteArrayListener
    {
        private ISerialPort Port { get; }

        public MeadowSerialListener(ISerialPort port, ICommandSerializer serializer)
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


            Console.WriteLine(" Listening for serial commands...");
            var t = Encoding.ASCII.GetBytes(".");
            while (true)
            {
                Thread.Sleep(1000);
                Port.Write(t);
                Console.WriteLine(".");

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
    }
}