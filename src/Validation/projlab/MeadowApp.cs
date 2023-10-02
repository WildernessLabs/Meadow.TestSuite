using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class MeadowApp : ValidationApp<F7FeatherV2>
    {
        private IDigitalOutputPort _red;
        private IDigitalOutputPort _green;
        private IDigitalOutputPort _blue;

        private ProjectLabTestDevice _testDevice;
        private MicroGraphics _graphics;

        public override Task Initialize()
        {
            Resolver.Log.Info($"Project Lab Validation");

            _red = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            _green = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);
            _blue = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);
            _testDevice = new ProjectLabTestDevice(Device, ProjectLab.Create());

            // create a display - just doing this verifies SPI
            _graphics = new MicroGraphics(_testDevice.ProjectLab.Display);
            _graphics.Rotation = RotationType._90Degrees;
            _graphics.CurrentFont = new Font12x20();

            return base.Initialize();
        }

        public override MeadowTestDevice DeviceUnderTest => _testDevice;

        public override void DisplayTestsRunning()
        {
            _graphics?.DrawRectangle(0, 0, _testDevice.ProjectLab.Display.Width, _testDevice.ProjectLab.Display.Height, Color.Yellow, true);
            _graphics.DrawText(_testDevice.ProjectLab.Display.Width / 2, _testDevice.ProjectLab.Display.Height / 2, "Running", color: Color.Black, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);

            _graphics.Show();
            _green.State = true;
            _red.State = true;
        }

        public override void DisplaySuccess()
        {
            _graphics?.DrawRectangle(0, 0, _testDevice.ProjectLab.Display.Width, _testDevice.ProjectLab.Display.Height, Color.Lime, true);
            _graphics.DrawText(_testDevice.ProjectLab.Display.Width / 2, _testDevice.ProjectLab.Display.Height / 2, "PASS", color: Color.Black, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);
            _graphics.Show();
            _green.State = true;
            _red.State = false;
            _blue.State = false;
        }

        public override void DisplayFailure()
        {
            _graphics?.DrawRectangle(0, 0, _testDevice.ProjectLab.Display.Width, _testDevice.ProjectLab.Display.Height, Color.Red, true);
            _graphics.DrawText(_testDevice.ProjectLab.Display.Width / 2, _testDevice.ProjectLab.Display.Height / 2, "FAIL", color: Color.White, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);
            _graphics.Show();
            _green.State = false;
            _red.State = true;
            _blue.State = false;
        }

        public override void OnExecutionHeartbeat()
        {
            _blue.State = !_blue.State;
            base.OnExecutionHeartbeat();
        }

        public override IEnumerable<ITest<MeadowTestDevice>> TestsToRun
        {
            get
            {
                return new ITest<ProjectLabTestDevice>[]
                {
//                    new I2CBusTest<ProjectLabTestDevice>(),
//                    new WiFiConnectionPositiveTest<ProjectLabTestDevice>(),
//                    new SpiBusTest(),
//                    new WiFiConnectionInvalidSsidTest<ProjectLabTestDevice>(),
                    new WiFiConnectionInvalidPasscodeTest<ProjectLabTestDevice>()
                }
                .Cast<ITest<MeadowTestDevice>>();
            }
        }
    }
}