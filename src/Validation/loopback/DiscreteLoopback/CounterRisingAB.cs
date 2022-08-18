using Meadow.Hardware;

namespace Validation
{
    public class CounterTwoEdgeAB : CounterBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeBoth, true);
        }
    }

    public class CounterTwoEdgeBA : CounterBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeBoth, false);
        }
    }

    public class CounterFallingAB : CounterBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeFalling, true);
        }
    }

    public class CounterFallingBA : CounterBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeFalling, false);
        }
    }

    public class CounterRisingAB : CounterBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeRising, true);
        }
    }
}