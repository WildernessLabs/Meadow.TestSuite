using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Validation;

public class MeadowApp : ValidationApp<F7CoreComputeV2, ProjectLabTestDevice>
{
    private IRgbPwmLed _led;

    private ProjectLabTestDevice _testDevice;
    private MicroGraphics _graphics;
    private bool _heartBeat;

    public override Task Initialize()
    {
        Resolver.Log.Info($"Project Lab Validation");

        _testDevice = new ProjectLabTestDevice(Device, ProjectLab.Create());
        _led = _testDevice.ProjectLab.RgbLed;

        // create a display - just doing this verifies SPI
        _graphics = new MicroGraphics(_testDevice.ProjectLab.Display);
        _graphics.Rotation = RotationType._270Degrees;
        _graphics.CurrentFont = new Font12x20();

        return base.Initialize();
    }

    public override ProjectLabTestDevice DeviceUnderTest => _testDevice;

    public override void DisplayTestsRunning()
    {
        _graphics?.DrawRectangle(0, 0, _testDevice.ProjectLab.Display.Width, _testDevice.ProjectLab.Display.Height, Color.Yellow, true);
        _graphics.DrawText(_testDevice.ProjectLab.Display.Width / 2, _testDevice.ProjectLab.Display.Height / 2, "Running", color: Color.Black, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);

        _graphics.Show();
        _led.SetColor(Color.Yellow);
    }

    public override void DisplaySuccess()
    {
        _graphics?.DrawRectangle(0, 0, _testDevice.ProjectLab.Display.Width, _testDevice.ProjectLab.Display.Height, Color.Lime, true);
        _graphics.DrawText(_testDevice.ProjectLab.Display.Width / 2, _testDevice.ProjectLab.Display.Height / 2, "PASS", color: Color.Black, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);
        _graphics.Show();
        _led.SetColor(Color.Green);
    }

    public override void DisplayFailure()
    {
        _graphics?.DrawRectangle(0, 0, _testDevice.ProjectLab.Display.Width, _testDevice.ProjectLab.Display.Height, Color.Red, true);
        _graphics.DrawText(_testDevice.ProjectLab.Display.Width / 2, _testDevice.ProjectLab.Display.Height / 2, "FAIL", color: Color.White, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);
        _graphics.Show();
        _led.SetColor(Color.Red);
    }

    public override void OnExecutionHeartbeat()
    {
        Resolver.Log.Info($"+heartbeat");

        _heartBeat = !_heartBeat;

        if (_heartBeat)
        {
            _led.SetColor(Color.Blue);
        }
        else
        {
            _led.SetColor(Color.Cyan);
        }
    }

    public override IEnumerable<ITest<ProjectLabTestDevice>> TestsToRun
    {
        get
        {
            Resolver.Log.Info($"Building test list...");
            Resolver.Log.Info($"Returning test list...");

            return new ITest<ProjectLabTestDevice>[]
            {
//                    new BluetoothTest<ProjectLabTestDevice>(),
//                    new I2CBusTest<ProjectLabTestDevice>(),
                    new WiFiConnectionPositiveTest<ProjectLabTestDevice>(),
//                    new SpiBusTest(),
//                    new WiFiConnectionInvalidSsidTest<ProjectLabTestDevice>(),
//                    new WiFiConnectionInvalidPasscodeTest<ProjectLabTestDevice>()
            };
        }
    }
}