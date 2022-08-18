using Meadow.Hardware;

namespace Validation
{
    public class RisingInterruptBA : InterruptBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeRising, true);
        }
    }
}