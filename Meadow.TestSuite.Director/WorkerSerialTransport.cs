using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading;
using Meadow.TestSuite;

namespace Meadow.TestSuite
{
    public class WorkerSerialTransport : TestTransportBase
    {
        private SerialPort SerialPort { get; }

        public const int ResponseTimeoutSeconds = 120;

        internal bool ExternalManageSerialPort { get; set; } = false;
        
        public WorkerSerialTransport(ICommandSerializer serializer, SerialPort serialPort)
            : base(serializer)
        {
            SerialPort = serialPort;
        }

        public WorkerSerialTransport(ICommandSerializer serializer, string serialPort, int baudRate = 9600)
            : base(serializer)
        {
            SerialPort = new SerialPort(serialPort, baudRate);
        }

        public override byte[] DeliverCommand(TestCommand command)
        {
            var data = Serializer.SerializeCommand(command).ToArray();

            if (!SerialPort.IsOpen)
            {
                SerialPort.Open();
            }

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
                index += c;
                toWrite -= c;

                progress += steps;
                if (progress > 100)
                {
                    // cap due to math
                    progress = 100f;
                }

                Debug.WriteLine($"{progress}%");
                
                // oddly sometimes (on first run since power?) this delay is required, but once it's worked once, it can be ignored.
                // no idea yet WTF is going on there.
                Thread.Sleep(30);
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

            // TODO: start a timeout - make this configurable
            SerialPort.ReadTimeout = ResponseTimeoutSeconds * 1000;

            // receive any result
            // get length
            var error = false;
            Array.Clear(length, 0, 4);

            var read = 0;
            while (read < 4)
            {
                try
                {
                    read += SerialPort.Read(length, read, 4 - read);
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("ERR: Timeout waiting on response");
                    error = true;
                    break;
                }
            }

            if (!error)
            {
                read = 0;
                var result = new byte[BitConverter.ToInt32(length)];

                while (read < result.Length)
                {
                    //get data

                    try
                    {
                        read += SerialPort.Read(result, read, result.Length - read);
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("ERR: Timeout waiting on response");
                        error = true;
                        break;
                    }
                }

                if (!error)
                {
                    return result;
                }
            }

            if (!ExternalManageSerialPort)
            {
                SerialPort.Close();
            }

            return null;
        }
    }
}
