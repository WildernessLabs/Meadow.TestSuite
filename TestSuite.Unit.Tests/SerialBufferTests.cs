using Meadow.TestSuite;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace TestSuite.Unit.Tests
{
    public class SerialBufferTests : ByteArrayListener
    {
        public SerialBufferTests()
            : base(new TestSerializer())
        {

        }

        [Fact]
        public void BufferTest1()
        {
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

            this.HandleData(input.ToArray(), 0, input.Count);

            // TODO: add something for testing.  m_buffer will be null because all data was received, but it wasn't an actual command
            Assert.Null(this.m_buffer);
        }

        [Fact]
        public void BufferTest2()
        {
            var r = new Random();
            var buffer = new byte[r.Next(10, 1024)];
            r.NextBytes(buffer);

            var input = new List<byte>();

            input.AddRange(new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55
            });
            input.AddRange(BitConverter.GetBytes(buffer.Length));

            this.HandleData(input.ToArray(), 0, input.Count);
            this.HandleData(buffer, 0, buffer.Length);

            // TODO: add something for testing.  m_buffer will be null because all data was received, but it wasn't an actual command
            Assert.Null(this.m_buffer);
        }

        [Fact]
        public void BufferTest3()
        {
            var commandReceived = false;

            var r = new Random();
            var buffer = new byte[r.Next(10, 1024)];
            r.NextBytes(buffer);

            this.CommandReceived += (l, c) =>
            {
                commandReceived = true;
            };

            var input = new List<byte>();

            input.AddRange(new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55
            });
            input.AddRange(BitConverter.GetBytes(buffer.Length));

            this.HandleData(input.ToArray(), 0, input.Count);
            var mid = buffer.Length / 2;
            this.HandleData(buffer, 0, mid);
            this.HandleData(buffer, mid, buffer.Length - mid);

            // TODO: add something for testing.  m_buffer will be null because all data was received, but it wasn't an actual command
            Assert.Null(this.m_buffer);
        }

        public override void SendResult(object result)
        {
            throw new NotImplementedException();
        }

        public override void StartListening()
        {
            throw new NotImplementedException();
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

            public T Deserialize<T>(ReadOnlySpan<byte> commandPayload)
            {
                throw new NotImplementedException();
            }

            public byte[] SerializeCommand(TestCommand command)
            {
                throw new NotImplementedException();
            }

            public byte[] SerializeResult(object result)
            {
                throw new NotImplementedException();
            }
        }
    }
}
