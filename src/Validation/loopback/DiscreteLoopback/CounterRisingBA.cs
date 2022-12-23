using Meadow.Hardware;

namespace Meadow.Validation
{
    public class CounterRisingBA : CounterBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeRising, false);
        }
    }
}