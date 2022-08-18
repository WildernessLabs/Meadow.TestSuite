using Meadow.TestSuite;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestSuite.Unit.Tests
{
    // NOTE: These tests require a serial port connected to a Meadow with the Worker running
    // They are intended to test the Director only
    public class DirectorUplinkTests
    {
        [Fact]
        public void UplinkAssemblyAndGetTests()
        {
            // inspect the assembly locally

            // uplink the assembly

            // ask the Worker what tests it found

            // make sure the lists match
        }
    }

    // NOTE: These tests require a serial port and that the TX and RX are tied together on that port
    public class SerializedTransferTests
    {
        [Fact]
        public void SystemTextRoundTripMed()
        {
            var port = new SerialPort("COM12", 9600);
            port.Open();
            var serializer = new CommandJsonSerializer();

            serializer.UseLibrary = JsonLibrary.SystemTextJson;

            var listener = new DesktopSerialListener(port, serializer);
            Task.Run(() => listener.StartListening());

            UplinkFileCommand rxCmd = null;

            listener.CommandReceived += (l, command) =>
            {
                rxCmd = command as UplinkFileCommand;
            };

            var transport = new WorkerSerialTransport(serializer, port);
            transport.ExternalManageSerialPort = true; // prevent the transport from closing the port
            var payload = new StringBuilder();
            for(int i = 0; i < 100; i++)
            {
                payload.Append($"{i}: abcdefghijklmnopqrstuvwxyz\r\n");
            }

            var txCmd = new UplinkFileCommand
            {
                Destination = "arbitrary path",
                FileData = payload.ToString()
            };

            Thread.Sleep(1000);
            transport.DeliverCommand(txCmd);
            Thread.Sleep(5000);
            Assert.NotNull(rxCmd);
            Assert.Equal(txCmd.Destination, rxCmd.Destination);
            Assert.Equal(txCmd.FileData, rxCmd.FileData);
        }

        [Fact]
        public void SystemTextRoundTripSmall()
        {
            var port = new SerialPort("COM12", 9600);
            port.Open();
            var serializer = new CommandJsonSerializer();

            serializer.UseLibrary = JsonLibrary.SystemTextJson;

            var listener = new DesktopSerialListener(port, serializer);
            Task.Run(() => listener.StartListening());

            UplinkFileCommand rxCmd = null;

            listener.CommandReceived += (l, command) =>
            {
                rxCmd = command as UplinkFileCommand;
            };

            var transport = new WorkerSerialTransport(serializer, port);
            transport.ExternalManageSerialPort = true; // prevent the transport from closing the port
            var txCmd = new UplinkFileCommand
            {
                Destination = "arbitrary path",
                FileData = "foo-bar-baz"
            };

            Thread.Sleep(1000);
            transport.DeliverCommand(txCmd);
            Thread.Sleep(2000);
            Assert.NotNull(rxCmd);
            Assert.Equal(txCmd.Destination, rxCmd.Destination);
            Assert.Equal(txCmd.FileData, rxCmd.FileData);
        }
    }
}
