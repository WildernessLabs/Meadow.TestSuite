﻿using Meadow.Devices;
using Meadow.Hardware;
using System;
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

        public override Task Initialize()
        {
            _red = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            _green = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);
            _blue = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);

            return base.Initialize();
        }

        public override MeadowTestDevice DeviceUnderTest => new MeadowF7TestDevice(Device);

        public override void DisplayTestsRunning()
        {
            _red.State = true;
            _green.State = true;
        }

        public override void DisplaySuccess()
        {
            _red.State = false;
            _green.State = true;
            _blue.State = false;
        }

        public override void DisplayFailure()
        {
            _red.State = true;
            _green.State = false;
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
                return new ITest<MeadowTestDevice>[]
                {
//                  new ReflectionTest(),
                  new BluetoothTest<MeadowTestDevice>(),
                  new WiFiAntennaSwitchingTest<MeadowTestDevice>(),
                  new FileSystemTest<MeadowTestDevice>(),
                  new SQLiteTest<MeadowTestDevice>(),
                  new WiFiSSLLoopTest<MeadowTestDevice>(),
                  new WiFiScanForAccessPointsTest<MeadowTestDevice>(),
                };
            }
        }
    }
}