using FeatherF7Test.Hardware;
using FeatherF7Test.Services;
using Meadow;
using Meadow.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;

using SoakTests;
using SoakTests.Common;

namespace FeatherF7Test;

// public class MeadowApp : App<F7FeatherV1>
public class MeadowApp : App<F7FeatherV2>
{
    /// <summary>
    /// OLED hardware object.  This object contains the OLED display and 8 LED objects.
    /// </summary>
    IOLEDBoardHardware _hardware;

    /// <summary>
    /// SSD1306 OLED display to show progress.
    /// </summary>
    DisplayController _displayService;

    /// <summary>
    /// Soak test configuration, this is read from the app.config.yaml file.
    /// </summary>
    SoakTestSettings _config;

    /// <summary>
    /// Test to be run.
    /// </summary>
    ISoakTest _test;

    /// <summary>
    /// Configure the application and run the test named in the app.config.yaml file.
    /// </summary>
    public override Task Initialize()
    {
        _config = new SoakTestSettings();

        _hardware = new OLEDBoardHardware();
        _hardware.Initialize(Device);

        _displayService = new DisplayController(_hardware.Display);
        _displayService.Clear();

        Helpers.WaitForNetworkConnection(Device);

        _test = RegisteredTests.GetTest(_config.TestName);
        if (_test == null)
        {
            _displayService.UpdateTitle("ERROR");
            _displayService.Log($"{_config.TestName}", false);
            _displayService.Log("not found.", false);
            Console.WriteLine($"Test '{_config.TestName}' not found.");
            while (true)
            {
                _hardware.Leds[7].IsOn = !_hardware.Leds[7].IsOn;
                Thread.Sleep(500);
            }
            
        }
        _displayService.UpdateTitle(_config.TestName);
        _test.Initialize(_config);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Run the specified test the specified number of times.
    /// </summary>
    public override Task Run()
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
                _displayService.Log($"{counter:N0}");
            }
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Executing test {counter:N0}");
            _test.Execute();
            if (_config.DelayBetweenCyclesMs > 0)
            {
                Thread.Sleep(_config.DelayBetweenCyclesMs);
            }
            _hardware.Leds[0].IsOn = !_hardware.Leds[0].IsOn;
        }
        _test.Teardown();

        _displayService.Log("Done.");
        Console.WriteLine("Done.");

        Thread.Sleep(Timeout.Infinite);

        return Task.CompletedTask;
    }
}
