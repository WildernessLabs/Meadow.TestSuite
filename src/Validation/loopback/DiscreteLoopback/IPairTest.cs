
using Meadow;
using System;
using System.Collections.Generic;

namespace Meadow.Validation
{
    public interface IPairTest
    {
        bool RunTest(PinPair pair);

        public bool RunTest(IEnumerable<PinPair> pairs)
        {
            var success = true;

            foreach(var pair in pairs)
            {
                try
                {
                    success &= RunTest(pair);
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