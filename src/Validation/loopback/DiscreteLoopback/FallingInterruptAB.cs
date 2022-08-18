using Meadow.Hardware;

namespace Validation
{
    public class FallingInterruptAB : InterruptBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeFalling, true);
        }
    }
}