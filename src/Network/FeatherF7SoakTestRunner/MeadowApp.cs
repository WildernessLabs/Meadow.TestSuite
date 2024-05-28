using Meadow;
using Meadow.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;

using Meadow.Foundation.Displays;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using Meadow.Units;

using DisplayControllers;
using SoakTests;
using SoakTests.Common;

namespace FeatherF7Test;

public class MeadowApp : App<F7FeatherV1>
// public class MeadowApp : App<F7FeatherV2>
{
    /// <summary>
    /// SSD1306 OLED display to show progress.
    /// </summary>
    IDisplayController _display;

    /// <summary>
    /// IPixelDisplay object that implements the hardware interface.
    /// </summary>
    IPixelDisplay _displayHardware;

    /// <summary>
    /// Soak test configuration, this is read from the app.config.yaml file.
    /// </summary>
    SoakTestSettings _config;

    /// <summary>
    /// Test to be run.
    /// </summary>
    ISoakTest _test;

    private void SetupHardware()
    {
        var spiBus = Device.CreateSpiBus(
            Device.Pins.SCK,
            Device.Pins.COPI,
            Device.Pins.CIPO,
            new Frequency(48000, Frequency.UnitType.Kilohertz));

        var chipSelectPort = Device.CreateDigitalOutputPort(Device.Pins.D05);
        var dcPort = Device.CreateDigitalOutputPort(Device.Pins.D14);
        var resetPort = Device.CreateDigitalOutputPort(Device.Pins.D15);

        Thread.Sleep(50);

        var display = new Ili9341(
            spiBus: spiBus,
            chipSelectPort: chipSelectPort,
            dataCommandPort: dcPort,
            resetPort: resetPort,
            width: 240, height: 320,
            colorMode: ColorMode.Format16bppRgb565)
        {
            SpiBusMode = SpiClockConfiguration.Mode.Mode3,
            SpiBusSpeed = new Frequency(48000, Frequency.UnitType.Kilohertz)
        };

        ((Ili9341) display).SetRotation(RotationType._270Degrees);

        _displayHardware = display;
    }

    /// <summary>
    /// Configure the application and run the test named in the app.config.yaml file.
    /// </summary>
    public override Task Initialize()
    {
        _config = new SoakTestSettings();

        SetupHardware();

        _display = new ILI9341DisplayController(_displayHardware);
        _display.Clear();

        SoakTests.Common.Helpers.WaitForNetworkConnection(Device);

        _test = RegisteredTests.GetTest(_config.TestName);
        if (_test == null)
        {
            _display.UpdateTitle("ERROR");
            _display.Log($"{_config.TestName}", false);
            _display.Log("not found.", false);
            Console.WriteLine($"Test '{_config.TestName}' not found.");
            while (true)
            {
                Thread.Sleep(500);
            }
            
        }
        _display.UpdateTitle(_config.TestName);
        _test.Initialize(_config);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Run the specified test the specified number of times.
    /// </summary>
    public override async Task Run()
    {
        //
        //  Now some variables used to show progress.
        //
        int counter = 0;
        int modulo = _config.NumberOfCycles switch
        {
            > 100 => 100,
            > 10 => 10,
            _ => 0
        };

        while (counter < _config.NumberOfCycles)
        {
            counter++;
            if ((modulo == 0) || (counter % modulo == 0) || (counter < 10))
            {
                _display.Log($"{counter:N0}");
            }
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Executing test {counter:N0}");
            await _test.Execute();
            if (_config.DelayBetweenCyclesMs > 0)
            {
                Thread.Sleep(_config.DelayBetweenCyclesMs);
            }
        }
        _test.Teardown();

        _display.Log("Done.");
        Console.WriteLine("Done.");

        Thread.Sleep(Timeout.Infinite);
    }
}
