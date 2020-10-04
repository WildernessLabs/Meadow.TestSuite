using Meadow.TestsSuite;
using MeadowApp;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xunit;

namespace TestSuite.Unit.Tests
{
    public class SerialBufferTests
    {
        [Fact]
        public void BufferTest1()
        {
            var listener = new SerialListener(null, null);

            var r = new Random();
            var buffer = new byte[r.Next(10, 1024)];
            r.NextBytes(buffer);

            var input = new List<byte>();

            input.AddRange(new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55
            });
            input.AddRange(BitConverter.GetBytes(buffer.Length));
            input.AddRange(buffer);

            listener.HandleData(input.ToArray(), 0, input.Count);
            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == buffer.Length);
            Assert.Equal(buffer, listener.m_buffer);
        }

        [Fact]
        public void BufferTest2()
        {
            var listener = new SerialListener(null, null);

            var r = new Random();
            var buffer = new byte[r.Next(10, 1024)];
            r.NextBytes(buffer);

            var input = new List<byte>();

            input.AddRange(new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55
            });
            input.AddRange(BitConverter.GetBytes(buffer.Length));

            listener.HandleData(input.ToArray(), 0, input.Count);
            listener.HandleData(buffer, 0, buffer.Length);

            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == buffer.Length);
            Assert.Equal(buffer, listener.m_buffer);
        }

        [Fact]
        public void BufferTest3()
        {
            var serializer = new TestSerializer();
            var listener = new SerialListener(null, serializer);
            var commandReceived = false;

            var r = new Random();
            var buffer = new byte[r.Next(10, 1024)];
            r.NextBytes(buffer);

            listener.CommandReceived += (c) =>
            {
                commandReceived = true;
            };

            var input = new List<byte>();

            input.AddRange(new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55
            });
            input.AddRange(BitConverter.GetBytes(buffer.Length));

            listener.HandleData(input.ToArray(), 0, input.Count);
            var mid = buffer.Length / 2;
            listener.HandleData(buffer, 0, mid);
            listener.HandleData(buffer, mid, buffer.Length - mid);

            Assert.True(commandReceived);
            Assert.NotNull(serializer.Data);
            Assert.Equal(buffer, serializer.Data);
        }

        class TestSerializer : ICommandSerializer
        {
            public byte[] Data { get; set; }

            public TestCommand Deserialize(ReadOnlySpan<byte> commandPayload)
            {
                Data = new byte[commandPayload.Length];
                commandPayload.CopyTo(Data);
                return null;
            }

            public byte[] Serialize(TestCommand command)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class SerialHeaderTests
    {
        [Fact]
        public void FindHeader1()
        {
            var listener = new SerialListener(null, null);

            var input = new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55,
                0x78, 0x56, 0x34, 0x12
            };

            listener.HandleData(input, 0, input.Length);
            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader2()
        {
            var listener = new SerialListener(null, null);

            var input1 = new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55
            };

            var input2 = new byte[]
            {
                0x78, 0x56, 0x34, 0x12
            };

            listener.HandleData(input1, 0, input1.Length);
            listener.HandleData(input2, 0, input2.Length);

            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader3()
        {
            var listener = new SerialListener(null, null);

            var input = new byte[]
            {
                0xde, 0xad, 0xbe, 0xef,
                0xaa, 0x55, 0xaa, 0x55,
                0x78, 0x56, 0x34, 0x12
            };

            listener.HandleData(input, 0, input.Length);
            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader4()
        {
            var listener = new SerialListener(null, null);

            var input0 = new byte[]
            {
                0xde, 0xad, 0xbe, 0xef
            };

            var input1 = new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55
            };

            var input2 = new byte[]
            {
                0x78, 0x56, 0x34, 0x12
            };

            listener.HandleData(input0, 0, input0.Length);
            listener.HandleData(input1, 0, input1.Length);
            listener.HandleData(input2, 0, input2.Length);

            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader5()
        {
            var listener = new SerialListener(null, null);

            var input0 = new byte[]
            {
                0xde, 0xad, 0xbe, 0xef, 0xaa
            };

            var input1 = new byte[]
            {
                0x55, 0xaa, 0x55, 0x78
            };

            var input2 = new byte[]
            {
                0x56, 0x34, 0x12
            };

            listener.HandleData(input0, 0, input0.Length);
            listener.HandleData(input1, 0, input1.Length);
            listener.HandleData(input2, 0, input2.Length);

            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader6()
        {
            var listener = new SerialListener(null, null);

            var input1 = new byte[]
            {
                0xaa
            };

            var input2 = new byte[]
            {
                0x55, 0xaa, 0x55, 0x78, 0x56, 0x34, 0x12
            };

            listener.HandleData(input1, 0, input1.Length);
            listener.HandleData(input2, 0, input2.Length);

            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader7()
        {
            var listener = new SerialListener(null, null);

            var input1 = new byte[]
            {
                0xaa, 0x55
            };

            var input2 = new byte[]
            {
                0xaa, 0x55, 0x78, 0x56, 0x34, 0x12
            };

            listener.HandleData(input1, 0, input1.Length);
            listener.HandleData(input2, 0, input2.Length);

            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader8()
        {
            var listener = new SerialListener(null, null);

            var input1 = new byte[]
            {
                0xaa, 0x55, 0xaa
            };

            var input2 = new byte[]
            {
                0x55, 0x78, 0x56, 0x34, 0x12
            };

            listener.HandleData(input1, 0, input1.Length);
            listener.HandleData(input2, 0, input2.Length);

            Assert.True(listener.m_buffer != null);
            Assert.True(listener.m_buffer.Length == 0x12345678);
        }
    }
}
