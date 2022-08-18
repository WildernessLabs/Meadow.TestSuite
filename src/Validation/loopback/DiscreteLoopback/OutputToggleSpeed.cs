
using Meadow;
using Meadow.Hardware;
using System.Diagnostics;

namespace Validation
{
    public class OutputToggleSpeed : IPinTest
    {
        public bool RunTest(IMeadowDevice device, IPin pin)
        {
            var op = device.CreateDigitalOutputPort(pin, true);
            var count = 1000;

            var stopwatch = Stopwatch.StartNew();

            for(int i = 0; i < count; i++)
            {
                op.State = false;
                op.State = true;
            }

            Resolver.Log.Info($"{count} pulses took {stopwatch.ElapsedMilliseconds}ms");

            // NOTE 0.6.6.6 result: 1000 pulses took 862ms ( 0.86 ms/pulse )
            var knownGood = 862;

            // look for regression of > 10%
            var success = stopwatch.ElapsedMilliseconds < knownGood * 1.1;
            if(!success)
            {
                Resolver.Log.Error($"Output toggle speed is out of expected range.");
            }

            return success;
        }
    }
}