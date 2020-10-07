using Meadow.TestSuite;
using MeadowApp;
using System.Windows.Input;
using Xunit;

namespace TestSuite.Unit.Tests
{
    public class SerialHeaderTests : ByteArrayListener
    {
        public SerialHeaderTests()
            : base(null)
        {

        }

        [Fact]
        public void FindHeader1()
        {
            var input = new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55,
                0x78, 0x56, 0x34, 0x12
            };

            this.HandleData(input, 0, input.Length);
            Assert.True(this.m_buffer != null);
            Assert.True(this.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader2()
        {
            var input1 = new byte[]
            {
                0xaa, 0x55, 0xaa, 0x55
            };

            var input2 = new byte[]
            {
                0x78, 0x56, 0x34, 0x12
            };

            this.HandleData(input1, 0, input1.Length);
            this.HandleData(input2, 0, input2.Length);

            Assert.True(this.m_buffer != null);
            Assert.True(this.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader3()
        {
            var input = new byte[]
            {
                0xde, 0xad, 0xbe, 0xef,
                0xaa, 0x55, 0xaa, 0x55,
                0x78, 0x56, 0x34, 0x12
            };

            this.HandleData(input, 0, input.Length);
            Assert.True(this.m_buffer != null);
            Assert.True(this.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader4()
        {
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

            this.HandleData(input0, 0, input0.Length);
            this.HandleData(input1, 0, input1.Length);
            this.HandleData(input2, 0, input2.Length);

            Assert.True(this.m_buffer != null);
            Assert.True(this.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader5()
        {
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

            this.HandleData(input0, 0, input0.Length);
            this.HandleData(input1, 0, input1.Length);
            this.HandleData(input2, 0, input2.Length);

            Assert.True(this.m_buffer != null);
            Assert.True(this.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader6()
        {
            var input1 = new byte[]
            {
                0xaa
            };

            var input2 = new byte[]
            {
                0x55, 0xaa, 0x55, 0x78, 0x56, 0x34, 0x12
            };

            this.HandleData(input1, 0, input1.Length);
            this.HandleData(input2, 0, input2.Length);

            Assert.True(this.m_buffer != null);
            Assert.True(this.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader7()
        {
            var input1 = new byte[]
            {
                0xaa, 0x55
            };

            var input2 = new byte[]
            {
                0xaa, 0x55, 0x78, 0x56, 0x34, 0x12
            };

            this.HandleData(input1, 0, input1.Length);
            this.HandleData(input2, 0, input2.Length);

            Assert.True(this.m_buffer != null);
            Assert.True(this.m_buffer.Length == 0x12345678);
        }

        [Fact]
        public void FindHeader8()
        {
            var input1 = new byte[]
            {
                0xaa, 0x55, 0xaa
            };

            var input2 = new byte[]
            {
                0x55, 0x78, 0x56, 0x34, 0x12
            };

            this.HandleData(input1, 0, input1.Length);
            this.HandleData(input2, 0, input2.Length);

            Assert.True(this.m_buffer != null);
            Assert.True(this.m_buffer.Length == 0x12345678);
        }

        public override void StartListening()
        {
            throw new System.NotImplementedException();
        }
    }
}
