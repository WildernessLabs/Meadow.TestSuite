using Meadow;
using Meadow.Hardware;
using Munit;
using System;
using System.Threading;

namespace ReleaseValidation.ProjectLab;

public class SerialPortTests
{
    // You can add this property and not use Resolver.Device if you prefer
    public IMeadowDevice Device { get; set; }

    [Fact]
    public void LoopbackTest()
    {
        var _random = new Random();
        using var serial = Device.CreateSerialPort(
            SerialPortName.Create("ttyS0", Device));

        serial.Open();

        var tx = new byte[1024];
        var rx = new byte[1024];
        _random.NextBytes(tx);

        serial.Write(tx);
        var read = 0;
        var timeout = 10;

        while (read < tx.Length)
        {
            read += serial.Read(rx, read, rx.Length - read);
            Thread.Sleep(100);
            if (timeout-- < 0) break;
        }

        // ReportInfo($"Sent {tx.Length} bytes.  Received {read} bytes");

        Assert.Equal(tx.Length, read);
    }

}
