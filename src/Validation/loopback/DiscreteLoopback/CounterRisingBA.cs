using Meadow.Hardware;

namespace Validation
{
    public class CounterRisingBA : CounterBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeRising, false);
        }
    }
}