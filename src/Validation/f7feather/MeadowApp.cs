using Meadow.Devices;
using Meadow.Hardware;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class MeadowApp : ValidationApp<F7FeatherV2> //  App<F7FeatherV2>
    {
        private IDigitalOutputPort redLed;
        private IDigitalOutputPort greenLed;

        public override Task Initialize()
        {
            redLed = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            greenLed = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);

            return base.Initialize();
        }

        public override MeadowTestDevice DeviceUnderTest => new MeadowF7TestDevice(Device);

        public override void DisplayTestsRunning()
        {
            redLed.State = true;
            greenLed.State = true;
        }

        public override void DisplaySuccess()
        {
            redLed.State = false;
            greenLed.State = true;
        }

        public override void DisplayFailure()
        {
            redLed.State = true;
            greenLed.State = false;
        }

        public override IEnumerable<ITest<MeadowTestDevice>> TestsToRun
        {
            get
            {
                return new ITest<MeadowF7TestDevice>[]
                {
                    new ReflectionTest(),
//                new BluetoothTest<MeadowF7TestDevice>(),
//                new WiFiAntennaSwitchingTest<MeadowF7TestDevice>(),
//                new FileSystemTest<MeadowF7TestDevice>(),
//                new SQLiteTest<MeadowF7TestDevice>(),
//                new WiFiSSLLoopTest<MeadowF7TestDevice>(),
//                new WiFiScanForAccessPointsTest<MeadowF7TestDevice>(),
                }
                .Cast<ITest<MeadowTestDevice>>();
            }
        }
    }
}