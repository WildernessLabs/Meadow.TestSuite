
using Meadow;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Validation
{
    public class SpiBusTest : ITest
    {
        public Task<bool> RunTest(IMeadowDevice device)
        {

            // TODO: connect to something on the bus to verify it's working

            return Task.FromResult(false);
        }
    }

    public class I2CBusTest : ITest
    {
        public Task<bool> RunTest(IMeadowDevice device)
        {

            // TODO: connect to something on the bus to verify it's working

            return Task.FromResult(false);
        }
    }

    public class WiFiConnectionPositiveTest : ITest
    {
        public async Task<bool> RunTest(IMeadowDevice device)
        {
            var wifi = device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            if (wifi == null) return false;

            try
            {
                await wifi.Connect("BOBS_YOUR_UNCLE", "1234567890");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to connect: {ex.Message}");

                return false;
            }

            // TODO: add some IP address testing, etc.

            return true;
        }
    }
}