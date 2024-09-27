using System;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class SleepTest<T> : ITest<T>
        where T : MeadowTestDevice
    {
        public async Task<bool> RunTest(T device)
        {
            Console.WriteLine("Starting sleep test...");

            (device as MeadowF7TestDevice).Device.PlatformOS.Sleep(TimeSpan.FromSeconds(10));

            return true;
        }
    }
}