using Meadow;
using Meadow.Hardware;
using System;
using System.Threading;

namespace ProjectLabBaseTests;

public class COM1LoopbackTest : TestBase
{
    private readonly Random _random = new Random();

    public override bool Execute(IMeadowDevice device)
    {
        using var serial = device.CreateSerialPort(
            SerialPortName.Create("ttyS0", device));

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

        ReportInfo($"Sent {tx.Length} bytes.  Received {read} bytes");

        if (tx.Length != read) return false;

        // TODO: compare the buffers

        return true;
    }
}
