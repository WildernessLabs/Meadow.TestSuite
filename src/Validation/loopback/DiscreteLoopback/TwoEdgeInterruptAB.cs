using Meadow.Hardware;

namespace Validation
{
    public class TwoEdgeInterruptAB : InterruptBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeBoth, true);
        }
    }

    public class TwoEdgeInterruptBA : InterruptBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeBoth, false);
        }
    }
}