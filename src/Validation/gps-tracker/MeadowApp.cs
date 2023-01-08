using Meadow.Devices;
using Meadow.Foundation.Graphics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private MicroGraphics _graphics;

        public override Task Initialize()
        {
            Resolver.Log.Info($"GPS Tracker Validation Init");

            return base.Initialize();
        }

        public async override Task Run()
        {
            Resolver.Log.Info($"GPS Tracker  Validation Run");

            var success = true;
            var failed = new List<string>();

            ShowInProgress();

            var tests = new ITest<MeadowTestDevice>[]
                {
                    new WiFiConnectionPositiveTest<MeadowTestDevice>(),
                    new WiFiWebRequestPositiveTest<MeadowTestDevice>(),
                    new WiFiScanPositiveTest<MeadowTestDevice>()
                };

            var complete = false;

            new Thread(() =>
            {
                while (!complete)
                {
                    Thread.Sleep(1000);
                }

            }).Start();

            foreach (var test in tests)
            {
                Resolver.Log.Info($"Running {test.GetType().Name}...");

                var result = await test.RunTest(new MeadowTestDevice(Device));

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
                Resolver.Log.Error(string.Join("\r\n ", failed));
            }
        }

        private void ShowInProgress()
        {
            //            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Yellow, true);
            //            _graphics.Show();
        }

        private void ShowSuccess()
        {
            //            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Green, true);
            //            _graphics.Show();
        }

        private void ShowFailed()
        {
            //            _graphics?.DrawRectangle(0, 0, _projectLab.Display.Width, _projectLab.Display.Height, Color.Red, true);
            //            _graphics.Show();
        }
    }
}