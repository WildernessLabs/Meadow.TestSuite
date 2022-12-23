﻿using Meadow.Hardware;

namespace Meadow.Validation
{
    public class FallingInterruptBA : InterruptBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeFalling, false);
        }
    }
}