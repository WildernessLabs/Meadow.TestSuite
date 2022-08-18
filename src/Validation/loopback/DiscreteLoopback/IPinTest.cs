
using Meadow;
using Meadow.Hardware;
using System;
using System.Collections.Generic;

namespace Validation
{
    public interface IPinTest
    {
        bool RunTest(IMeadowDevice device, IPin pin);

        public bool RunTest(IMeadowDevice device, IEnumerable<IPin> pins)
        {
            var success = true;

            foreach(var pin in pins)
            {
                try
                {
                    success &= RunTest(device, pin);
                }
                catch(Exception ex)
                {
                    Resolver.Log.Error($"test failure: {ex.Message}");
                    success = false;
                }
            }

            return success;
        }
    }
}