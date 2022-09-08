
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Validation
{
    public class WiFiConnectionPositiveTest : ITest
    {
        public async Task<bool> RunTest(IMeadowDevice device, ProjectLab projectLab)
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