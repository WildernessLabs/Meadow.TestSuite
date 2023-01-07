﻿using Meadow.Hardware;

namespace Meadow.Validation
{
    public class RisingInterruptAB : InterruptBase
    {
        public override bool RunTest(PinPair pair)
        {
            return base.RunTest(pair, InterruptMode.EdgeRising, true);
        }
    }
}