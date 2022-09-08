
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Validation
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private IDigitalOutputPort _red;

        private MicroGraphics _graphics;
        private St7789 _display;

        public override Task Initialize()
        {
            _red = Device.CreateDigitalOutputPort(Device.Pins.D00);

            return base.Initialize();
        }

        public override Task Run()
        {
            _red.State = true;

            Resolver.Log.Info($"Starting validation tests...");

            var wired = Device.NetworkAdapters.Primary<IWiredNetworkAdapter>();
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            var success = true;

            try
            {
                // create a display - just doing this verifies SPI
                //                _display = new St7789(
                //                _graphics = new MicroGraphics(_projectLab.Display);
            }
            catch(Exception ex)
            {
                Resolver.Log.Error($"Failed creating Display.");
                success = false;
            }

            ShowInProgress();

            Thread.Sleep(3000);

            Resolver.Log.Info($"Tests complete.");

            if(success)
            {
                ShowSuccess();
            }
            else
            {
                ShowFailed();
                Resolver.Log.Error("---- FAILED TESTS----");
                //                Resolver.Log.Error(string.Join("/r/n ", failed));
            }

            return base.Run();
        }

        private void ShowInProgress()
        {
            //            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Yellow, true);
            //            _graphics.Show();
            _red.State = true;
        }

        private void ShowSuccess()
        {
            //            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Green, true);
            //            _graphics.Show();
            _red.State = true;
        }

        private void ShowFailed()
        {
            //            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Red, true);
            //            _graphics.Show();
            _red.State = true;
        }
    }
}