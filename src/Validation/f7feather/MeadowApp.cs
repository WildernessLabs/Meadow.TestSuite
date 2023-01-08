using Meadow.Devices;
using Meadow.Hardware;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class MeadowApp : App<F7FeatherV2>
    {
        private IDigitalOutputPort redLed;
        private IDigitalOutputPort greenLed;

        public override Task Initialize()
        {
            redLed = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            greenLed = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);

            return base.Initialize();
        }

        public override async Task Run()
        {
            redLed.State = true;
            greenLed.State = true;

            Resolver.Log.Info($"Starting validation tests...");

            var success = true;

            var failed = new List<string>();

            var tests = new ITest<MeadowF7TestDevice>[]
            {
                new ReflectionTest(),

//                new BluetoothTest<MeadowF7TestDevice>(),
//                new WiFiAntennaSwitchingTest<MeadowF7TestDevice>(),
//                new FileSystemTest<MeadowF7TestDevice>(),
//                new SQLiteTest<MeadowF7TestDevice>(),
//                new WiFiSSLLoopTest<MeadowF7TestDevice>(),
//                new WiFiScanForAccessPointsTest<MeadowF7TestDevice>(),
            };

            foreach (var test in tests)
            {
                Resolver.Log.Info($"Running {test.GetType().Name}...");

                var result = await test.RunTest(new MeadowF7TestDevice(Device));

                if (!result)
                {
                    failed.Add(test.GetType().Name);
                }

                success &= result;
            }

            Resolver.Log.Info($"Tests complete.");

            if (success)
            {
                redLed.State = false;
            }
            else
            {
                greenLed.State = false;
                Resolver.Log.Error("---- FAILED TESTS----");
                Resolver.Log.Error(string.Join("/r/n ", failed));
            }
        }
    }
}