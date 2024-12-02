using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using ProjectLabTest.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

using SoakTests;
using SoakTests.Common;

namespace ProjLabV3SoakTestRunner;

public class MeadowApp : App<F7CoreComputeV2>
{
    /// <summary>
    /// Hardware we are running on.
    /// </summary>
    private IProjectLabHardware _projectLab;

    /// <summary>
    /// Onboard LED used for status indication.
    /// </summary>
    IRgbPwmLed _onboardLed;

    /// <summary>
    /// IL9341 display used to show test progress.
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

    public override Task Initialize()
    {
        Helpers.DeviceUnderTest = Device;

        _config = new SoakTestSettings();

        _projectLab = ProjectLab.Create();

        _onboardLed = _projectLab.RgbLed;
        _onboardLed.SetColor(Color.White);

        _displayService = new DisplayController(_projectLab.Display);

        _test = RegisteredTests.GetTest(_config.TestName);
        if (_test == null)
        {
            _onboardLed.SetColor(Color.Red);
            _displayService.UpdateTitle("ERROR");
            _displayService.Log($"{_config.TestName} not found", false);
            Console.WriteLine($"Test '{_config.TestName}' not found.");
            while (true)
            {
                Thread.Sleep(500);
            }
            
        }
        _displayService.UpdateTitle(_config.TestName);
        _displayService.Log("Connecting to network...");

        _test.Initialize(_config);

        return base.Initialize();
    }

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
            if ((counter % 2) == 0)
            {
                _onboardLed.SetColor(Color.Blue);
            }
            else
            {
                _onboardLed.SetColor(Color.Black);
            }
        }
        _test.Teardown();

        _onboardLed.SetColor(Color.Green);

        _displayService.Log("Done.");
        Console.WriteLine("Done.");

        Thread.Sleep(Timeout.Infinite);

        return base.Run();
    }
}