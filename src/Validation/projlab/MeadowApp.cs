
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Validation
{
    public class MeadowApp : App<F7FeatherV2>
    {
        private IDigitalOutputPort _red;
        private IDigitalOutputPort _green;

        private ProjectLab _projectLab;
        private MicroGraphics _graphics;

        public override Task Initialize()
        {
            Resolver.Log.Info($"Project Lab Validation Init");

            _red = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            _green = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);
            _projectLab = new ProjectLab();

            return base.Initialize();
        }

        public override async Task Run()
        {
            Resolver.Log.Info($"Project Lab Validation Run");

            _red.State = true;
            _green.State = true;

            Resolver.Log.Info($"Starting validation tests...");

            var success = true;

            try
            {
                // create a display - just doing this verifies SPI
                _graphics = new MicroGraphics(_projectLab.Display);
                _graphics.Rotation = RotationType._90Degrees;
                _graphics.CurrentFont = new Font12x20();
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed creating Display.");
                success = false;
            }

            ShowInProgress();

            var failed = new List<string>();

            var tests = new ITest[]
                {
//                    new I2CBusTest(),
//                    new WiFiConnectionPositiveTest(),
//                    new SpiBusTest(),
//                    new WiFiConnectionInvalidSsidTest(),
                    new WiFiConnectionInvalidPasscodeTest()
                };

            var complete = false;

            new Thread(() =>
            {
                while (!complete)
                {
                    _red.State = !_red.State;
                    Thread.Sleep(1000);
                }

            }).Start();

            foreach (var test in tests)
            {
                Resolver.Log.Info($"Running {test.GetType().Name}...");

                var result = await test.RunTest(Device, _projectLab);

                if (!result)
                {
                    failed.Add(test.GetType().Name);
                }

                success &= result;
            }

            Resolver.Log.Info($"Tests complete.");

            complete = true;

            if (success)
            {
                ShowSuccess();
            }
            else
            {
                ShowFailed();
                Resolver.Log.Error("---- FAILED TESTS----");
                Resolver.Log.Error(string.Join("/r/n ", failed));
            }
        }

        private void ShowInProgress()
        {
            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Yellow, true);
            _graphics.DrawText(_projectLab.Display.Width / 2, _projectLab.Display.Height / 2, "Running", color: Color.Black, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);

            _graphics.Show();
            _green.State = true;
            _red.State = true;
        }

        private void ShowSuccess()
        {
            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Lime, true);
            _graphics.DrawText(_projectLab.Display.Width / 2, _projectLab.Display.Height / 2, "PASS", color: Color.Black, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);
            _graphics.Show();
            _green.State = true;
            _red.State = false;
        }

        private void ShowFailed()
        {
            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Red, true);
            _graphics.DrawText(_projectLab.Display.Width / 2, _projectLab.Display.Height / 2, "FAIL", color: Color.White, alignmentH: HorizontalAlignment.Center, alignmentV: VerticalAlignment.Center);
            _graphics.Show();
            _green.State = false;
            _red.State = true;
        }
    }
}