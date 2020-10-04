using Meadow.Hardware;
using Meadow.TestsSuite;
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Text;
using System.Threading;

namespace MeadowApp
{
    public class SerialListener : ITestListener
    {
        public event CommandReceivedHandler CommandReceived;

        private Timer m_rxTimeoutTimer;
        internal int m_remaining = 0;
        internal byte[] m_buffer;
        internal List<byte> m_header = new List<byte>();
        private object m_handlerSyncRoot = new object();

        public const int CommandTimeoutSeconds = 5;

        private ISerialPort Port { get; }
        private ICommandSerializer Serializer { get; }

        public SerialListener(ISerialPort port, ICommandSerializer serializer)
        {
            Port = port;

            Serializer = serializer;            

            m_rxTimeoutTimer = new Timer(ReceiveTimeoutTimerProc, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void StartListening()
        {
            Console.WriteLine(" Opening serial port...");
            try
            {
                Port.Open();
            }
            catch(Exception ex)
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

                if(Port.BytesToRead > 0)
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
//            Console.WriteLine($"RX {Port.BytesToRead} bytes");

            if (Port.BytesToRead <= 0) return;

            var b = new byte[Port.BytesToRead];
            Port.Read(b, 0, b.Length);
            HandleData(b, 0, b.Length);
        }

        internal void HandleData(byte[] data, int offset, int length)
        {
            lock (m_handlerSyncRoot)
            {
                if (m_remaining <= 0)
                {
                    var index = offset;
                    var needForHeader = 8 - m_header.Count;
                    var remain = needForHeader < length ? needForHeader : length;

                    while (m_header.Count < 8)
                    {
                        if (index >= data.Length) return;

                        switch (m_header.Count)
                        {
                            case 0:
                                // look for 0xaa55aa55
                                while (data[index] != 0xaa)
                                {
                                    remain--;

                                    if (++index > data.Length - 1)
                                    {
                                        return;
                                    }
                                }

                                // we know we're on a 0xaa, but does the pattern match?
                                switch (remain - 1)
                                {
                                    case 0:
                                        m_header.Add(0xaa);
                                        return;
                                    case 1:
                                        if (data[index + 1] == 0x55)
                                        {
                                            m_header.Add(0xaa);
                                            m_header.Add(0x55);
                                            return;
                                        }
                                        index++;
                                        break;
                                    case 2:
                                        if ((data[index + 1] == 0x55)
                                            && (data[index + 2] == 0xaa))
                                        {
                                            m_header.Add(0xaa);
                                            m_header.Add(0x55);
                                            m_header.Add(0xaa);
                                            return;
                                        }
                                        index++;
                                        break;
                                    default:
                                        if ((data[index + 1] == 0x55)
                                            && (data[index + 2] == 0xaa)
                                            && (data[index + 3] == 0x55))
                                        {
                                            m_header.Add(0xaa);
                                            m_header.Add(0x55);
                                            m_header.Add(0xaa);
                                            m_header.Add(0x55);

                                            index += 4;
                                        }
                                        else
                                        {
                                            index++;
                                        }
                                        break;
                                }

                                break;
                            case 1:
                                // look for 0x55aa55
                                if (data[index] != 0x55)
                                {
                                    // we had an 0xaa, but it was not part of the pattern so restart
                                    m_header.Clear();

                                    HandleData(data, index, data.Length - index);
                                    return;
                                }

                                // we know we're on a 0x55, but does the pattern match?
                                switch (remain)
                                {
                                    case 0:
                                        m_header.Add(0x55);
                                        return;
                                    case 1:
                                        if (data[index + 1] == 0xaa)
                                        {
                                            m_header.Add(0x55);
                                            m_header.Add(0xaa);
                                            return;
                                        }
                                        index++;
                                        break;
                                    default:
                                        if ((data[index + 1] == 0xaa)
                                            && (data[index + 2] == 0x55))
                                        {
                                            m_header.Add(0x55);
                                            m_header.Add(0xaa);
                                            m_header.Add(0x55);

                                            index += 3;
                                        }
                                        else
                                        {
                                            index++;
                                        }
                                        break;
                                }

                                break;
                            case 2:
                                // look for 0xaa55
                                if (data[index] != 0xaa)
                                {
                                    // we had an 0xaa55, but it was not part of the pattern so restart
                                    m_header.Clear();

                                    HandleData(data, index, data.Length - index);
                                    return;
                                }

                                // we know we're on a 0xaa, but does the pattern match?
                                switch (remain)
                                {
                                    case 0:
                                        m_header.Add(0xaa);
                                        return;
                                    default:
                                        if (data[index + 1] == 0x55)
                                        {
                                            m_header.Add(0xaa);
                                            m_header.Add(0x55);

                                            index += 2;
                                        }
                                        else
                                        {
                                            index++;
                                        }
                                        break;
                                }

                                break;
                            case 3:
                                // look for 0x55
                                if (data[index] != 0x55)
                                {
                                    // we had an 0xaa55aa, but it was not part of the pattern so restart
                                    m_header.Clear();

                                    HandleData(data, index, data.Length - index);
                                    return;
                                }

                                // we know we're on a 0x55 so it's a match
                                switch (remain)
                                {
                                    case 0:
                                        m_header.Add(0x55);
                                        return;
                                    default:
                                        m_header.Add(0x55);
                                        index++;
                                        break;
                                }

                                break;
                            default:
                                m_header.Add(data[index++]);
                                break;

                        }
                    }

                    if (m_header.Count == 8)
                    {
                        var totalLength = BitConverter.ToInt32(m_header.ToArray(), 4);
                        Console.WriteLine($"Incoming packet expected to be {totalLength} bytes. Allocating...");
                        m_buffer = new byte[BitConverter.ToInt32(m_header.ToArray(), 4)];
                        Console.WriteLine($"Allocated.");

                        m_rxTimeoutTimer.Change(CommandTimeoutSeconds * 1000, Timeout.Infinite);

                        // any remaining data copy to the buffer
                        var count = length - index;
                        if (count > 0)
                        {
                            Array.Copy(data, index, m_buffer, 0, count);
                        }
                        m_remaining = m_buffer.Length - count;

                        if (m_remaining == 0)
                        {
                            ProcessMessageInBuffer();
                        }
                    }
                }
                else
                {
                    var copyCount = m_remaining < length ? m_remaining : length;
                    Array.Copy(data, offset, m_buffer, m_buffer.Length - m_remaining, copyCount);
                    m_remaining -= copyCount;

                    if (m_remaining == 0)
                    {
                        ProcessMessageInBuffer();
                    }
                    else
                    {
                        //reset the timer
                        m_rxTimeoutTimer.Change(CommandTimeoutSeconds * 1000, Timeout.Infinite);
                    }

                    if (copyCount < length)
                    {
                        // there was data from a subsequent packet
                        HandleData(data, copyCount, length - copyCount);
                    }
                }
            }
        }

        private void ProcessMessageInBuffer()
        {
            // stop the timer
            m_rxTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);

            // we've finished the packet
            Console.WriteLine("Command received!");
            TestCommand c = null;
            try
            {
                c = Serializer?.Deserialize(m_buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to deserialize: {ex.Message}");
            }
            if (c != null)
            {
                CommandReceived?.Invoke(c);
            }

            m_buffer = null;
            m_header.Clear();
        }

        private void ReceiveTimeoutTimerProc(object o)
        {
            Console.WriteLine($"Timeout waiting for command data.  Still waiting on {m_remaining} bytes.");
            m_remaining = 0;
            m_header.Clear();
            m_buffer = null;
        }
    }
}