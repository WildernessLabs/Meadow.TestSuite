using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestRecorder;
using TestRecorder.Shared.Messages;

namespace Validation;

public class TestService
{
    private IProjectLabHardware _hardware;
    private DisplayService _display;
    private TestResult? _lastResult = null;
    private List<TestInfo> _testStore = new();

    public TestService(IProjectLabHardware hardware)
    {
        _hardware = hardware;
        Initialize();
    }

    public async Task StartTests()
    {
        await RunTest(new SpiWriteTest(_hardware));
        await RunTest(new AccelerometerTest(_hardware));
        await RunTest(new LightSensorTest(_hardware));
        await RunTest(new TempSensorTest(_hardware));
        await RunTest(new DigitalOutputTest(_hardware));
        await RunTest(new DigitalInputHighTest(_hardware));
        await RunTest(new DigitalInputLowTest(_hardware));

        _display.ShowResults(_testStore);
    }

    private void DisplayResultClicked(object? sender, TestResult e)
    {
        _lastResult = e;
    }

    private void Initialize()
    {
        (_hardware.Display as Ili9341).InvertDisplay(false);

        _display = new DisplayService(
            new DisplayScreen(
                _hardware.Display,
                Meadow.Peripherals.Displays.RotationType._270Degrees,
                _hardware.Touchscreen)
            );

        _display.CreateValidationControls(
            _hardware.LeftButton,
            _hardware.RightButton,
            _hardware.DownButton);

        _display.ResultClicked += DisplayResultClicked;
        _display.PublishClicked += PublishResultsClicked;
    }

    private async Task RunTest(TestDescriptor test)
    {
        _display.SetTestName(test.TestName);
        _display.SetQuestionText(test.QuestionText);

        void UpdateInputlabel(object s, string label)
        {
            _display.SetInputsLabel(label);
        }

        _lastResult = null;

        test.UpdateInputs += UpdateInputlabel;
        test.BeginTest();

        while (_lastResult == null)
        {
            await Task.Delay(1000);
        }

        test.ResultReceived();

        AddTestInfo(new TestInfo(test.TestName, _lastResult.Value));
        _display.ClearTestLabels();

        test.UpdateInputs -= UpdateInputlabel;
    }

    public void AddTestInfo(TestInfo info)
    {
        _testStore.Add(info);
    }

    private async void PublishResultsClicked(object sender, EventArgs e)
    {
        _display.ShowStatus("Connecting to Cloud...");
        await Task.Delay(100);
        _display.HidePublishButton();

        // TODO: verify network connection

        var user = Resolver.App.Settings["TestRecorder.User"];
        var password = Resolver.App.Settings["TestRecorder.Password"];

        // TODO: verify user/password exist

        var sw = new Stopwatch();
        sw.Start();
        var client = new TestRecorder.Client(user, password);
        sw.Stop();
        Resolver.Log.Info($"Connecting took {sw.ElapsedMilliseconds} ms");

        // does the test exist in TestRecorder?
        var allTests = await client.GetAllTests();
        Resolver.Log.Info($"There are {allTests.Count()} existing tests");

        var sent = 0;

        foreach (var test in _testStore)
        {
            if (test.Result == TestResult.Skip) continue;

            Guid testID;

            var existing = allTests.FirstOrDefault(t => string.Compare(t.TestName, test.Name, true) == 0);

            if (existing == null)
            {
                foreach (var t in allTests)
                {
                    Resolver.Log.Info($"  - {t.TestName}");
                }

                Resolver.Log.Info($"Creating new test for {test.Name}...");

                // create the test
                testID = await client.CreateNewTest(test.Name, "Manual ProjLab Test", TargetPlatform.ProjLab);
            }
            else
            {
                testID = existing.TestID.Value;
            }

            // publish the result
            var result = new TestResultMessage
            {
                TestID = testID,
                TestName = test.Name,
                TestRunBy = "Manual ProjectLab",
                TargetPlatform = TargetPlatform.ProjLab,
                CompletedTimestamp = DateTime.UtcNow,
                StartedTimestamp = DateTime.UtcNow,
                State = test.Result.ToString(),
                MeadowOSVersion = _hardware.ComputeModule.Information.OSVersion,
                TargetInfo = $"ProjectLab {_hardware.RevisionString}"
            };

            _display.ShowStatus($"Publishing '{test.Name}'...");

            try
            {
                await client.PublishTestResult(result);
                sent++;
            }
            catch (Exception ex)
            {
                _display.ShowStatus($"Failed to publish test result: {ex.Message}");
                Resolver.Log.Error($"Failed to publish test result: {ex.Message}");
            }
        }

        _display.ShowStatus($"Done. {sent} results sent.");
    }

    /*

    public async Task<TestResult> TestGpioInputs(bool pullUp)
    {
        var resistor = pullUp ? ResistorMode.InternalPullUp : ResistorMode.InternalPullDown;

        using var d0 = _hardware.Gpio.Pins.D00.CreateDigitalInputPort(resistor);
        using var d1 = _hardware.Gpio.Pins.D01.CreateDigitalInputPort(resistor);
        using var d2 = _hardware.Gpio.Pins.D02.CreateDigitalInputPort(resistor);
        using var d3 = _hardware.Gpio.Pins.D03.CreateDigitalInputPort(resistor);
        using var d4 = _hardware.Gpio.Pins.D04.CreateDigitalInputPort(resistor);
        using var d5 = _hardware.Gpio.Pins.D05.CreateDigitalInputPort(resistor);
        using var d6 = _hardware.Gpio.Pins.D06.CreateDigitalInputPort(resistor);
        using var d7 = _hardware.Gpio.Pins.D07.CreateDigitalInputPort(resistor);

        if (pullUp)
        {
            _display.SetTestName("GPIO Input to Ground");
        }
        else
        {
            _display.SetTestName("GPIO Input to 3.3V");
        }
        _display.SetQuestionText("Are all inputs working?");

        while (_lastResult == null)
        {
            string s = string.Empty;
            s += d0.State ? "1" : "0";
            s += d1.State ? "1" : "0";
            s += d2.State ? "1" : "0";
            s += d3.State ? "1" : "0";
            s += " ";
            s += d4.State ? "1" : "0";
            s += d5.State ? "1" : "0";
            s += d6.State ? "1" : "0";
            s += d7.State ? "1" : "0";

            _display.SetInputsLabel(s);

            await Task.Delay(1000);
        }

        return _lastResult.Value;
    }

    */
}
