using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Meadow.TestsSuite;

namespace Meadow.TestSuite
{
    public class WorkerSerialTransport<TSerializer> : TestTransportBase<TSerializer>
        where TSerializer : ICommandSerializer
    {
        private SerialPort SerialPort { get; }

        public WorkerSerialTransport(string serialPort, int baudRate = 9600)
        {
            SerialPort = new SerialPort(serialPort, baudRate);

            (Serializer as CommandJsonSerializer).UseLibrary = CommandJsonSerializer.JsonLibrary.SystemTextJson;
        }

        public override void DeliverCommand(TestCommand command)
        {
            var data = Serializer.Serialize(command).ToArray();

            SerialPort.Open();

            // chunk the send since meadow has limited resources

            // first send a header with a known delimiter and then the length 
            var header = new byte[] { 0xaa, 0x55, 0xaa, 0x55 };
            SerialPort.Write(header, 0, header.Length);
            var length = BitConverter.GetBytes(data.Length);
            SerialPort.Write(length, 0, length.Length);

            Debug.WriteLine($"Sending {data.Length} bytes");

            var toWrite = data.Length;
            var index = 0;
            var bufferSize = 255;

            var steps = 100f / (float)(toWrite / bufferSize);
            var progress = 0f;
            
            var start = Environment.TickCount;

            while(toWrite > 0)
            {
                var c = toWrite > bufferSize ? bufferSize : toWrite;

                SerialPort.Write(data , index, c);
                toWrite -= c;

                progress += steps;
                Debug.WriteLine($"{progress}%");
                
                // oddly sometimes (on first run since power?) this delay is required, but once it's worked once, it can be ignored.
                // no idea yet WTF is going on there.
//                Thread.Sleep(100);
            }
            Debug.WriteLine($"100%");
            var et = Environment.TickCount - start;
            if (et <= 0)
            {
                Debug.WriteLine($"Transfer too fast to measure");
            }
            else
            {
                var throughput = data.Length * 1000 / et;
                Debug.WriteLine($"Effective throughput: {throughput}B/sec");
            }
            SerialPort.Close();

        }
    }
}
