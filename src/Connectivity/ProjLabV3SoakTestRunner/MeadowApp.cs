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

    /// <summary>
    /// Display a message on the IL9341 display.
    /// </summary>
    /// <param name="message">Message to be displayed.</param>
    public void LogToDisplay(string message)
    {
        _displayService.Log(message);
    }

    /// <summary>
    /// Initialise the application.
    /// </summary>
    public override Task Initialize()
    {
        Helpers.DeviceUnderTest = Device;

        _config = new SoakTestSettings();

        _projectLab = ProjectLab.Create();

        _onboardLed = _projectLab.RgbLed;
        _onboardLed.SetColor(Color.White);

        _displayService = new DisplayController(_projectLab.Display);
        Helpers.DisplayLogMessage = LogToDisplay;

        _test = RegisteredTests.GetTest(_config.TestName);
        if (_test == null)
        {
            _onboardLed.SetColor(Color.Red);
            _displayService.UpdateTitle("ERROR");
            LogToDisplay($"{_config.TestName} not found");
            Console.WriteLine($"Test '{_config.TestName}' not found.");
            while (true)
            {
                Thread.Sleep(500);
            }
            
        }
        _displayService.UpdateTitle(_config.TestName);
        LogToDisplay("Connecting to network...");

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

        //
        //  For a soak test we will run for a predetermined number of cycles as specified in the configuration file.
        //  Other tests will run once and it will be up to the test how long this test runs.  An example of this is
        //  is the Bluetooth test application that sends multiple values to the managed code and ends the test by
        //  sending a value of 0.
        //
        int numberOfCycles = (_config.TestName.ToLower().EndsWith("soaktest")) ? _config.NumberOfCycles : 1;

        while (counter < numberOfCycles)
        {
            counter++;
            if ((numberOfCycles > 1) && ((modulo == 0) || (counter % modulo == 0) || (counter < 10)))
            {
                LogToDisplay($"{counter:N0}");
                Helpers.ConsoleLog($"Executing test {counter:N0}");
            }
            else
            {
                if (numberOfCycles == 1)
                {
                    LogToDisplay("Executing test");
                    Helpers.ConsoleLog("Executing test");
                }
            }
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

        LogToDisplay("Done.");
        Helpers.ConsoleLog("Done.");

        Thread.Sleep(Timeout.Infinite);

        return base.Run();
    }
}