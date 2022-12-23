
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class MeadowApp : App<F7FeatherV2>
    {
        private IDigitalOutputPort _red;
        private IDigitalOutputPort _green;

        private PinPair[] _pairs;

        public override Task Initialize()
        {
            _red = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            _green = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);

            _pairs = new PinPair[]
            {
                new PinPair(Device, Device.Pins.A00, Device.Pins.A01),
                new PinPair(Device, Device.Pins.A02, Device.Pins.A03),
                new PinPair(Device, Device.Pins.A04, Device.Pins.A05),
                new PinPair(Device, Device.Pins.SCK, Device.Pins.COPI),
                new PinPair(Device, Device.Pins.CIPO, Device.Pins.D00),
                new PinPair(Device, Device.Pins.D01, Device.Pins.D02),
                new PinPair(Device, Device.Pins.D03, Device.Pins.D04),
                new PinPair(Device, Device.Pins.D05, Device.Pins.D06),
                new PinPair(Device, Device.Pins.D07, Device.Pins.D08),
                new PinPair(Device, Device.Pins.D09, Device.Pins.D10),
                new PinPair(Device, Device.Pins.D11, Device.Pins.D12),
                new PinPair(Device, Device.Pins.D13, Device.Pins.D14),
            };
            return base.Initialize();
        }

        public override Task Run()
        {
            _red.State = true;
            _green.State = true;

            Resolver.Log.Info($"Starting validation tests...");

            var success = true;

            var pinTests = new OutputToggleSpeed();
            success &= pinTests.RunTest(Device, Device.Pins.D00);

            var failed = new List<string>();

            var pairTests = new IPairTest[]
                {
                    new UniDirectionAB(),
                    new UniDirectionBA(),
                    new RisingInterruptAB(),
                    new RisingInterruptBA(),
                    new FallingInterruptAB(),
                    new FallingInterruptBA(),
                    new TwoEdgeInterruptAB(),
                    new TwoEdgeInterruptBA(),
                    new CounterRisingAB(),
                    new CounterRisingAB(),
                    new CounterFallingAB(),
                    new CounterFallingBA(),
                    new CounterTwoEdgeAB(),
                    new CounterTwoEdgeBA(),
                    new PushButtonAB(),
                    new PushButtonBA(),
                };

            foreach(var test in pairTests)
            {
                Resolver.Log.Info($"Running {test.GetType().Name}...");

                var result = test.RunTest(_pairs);

                if(!result)
                {
                    failed.Add(test.GetType().Name);
                }

                success &= result;
            }

            Resolver.Log.Info($"Tests complete.");

            if(success)
            {
                _red.State = false;
            }
            else
            {
                _green.State = false;
                Resolver.Log.Error("---- FAILED TESTS----");
                Resolver.Log.Error(string.Join("/r/n ", failed));
            }

            return base.Run();
        }
    }
}