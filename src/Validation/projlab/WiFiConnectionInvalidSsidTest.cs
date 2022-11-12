
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Validation
{
    public class WiFiConnectionInvalidSsidTest : ITest
    {
        public async Task<bool> RunTest(IMeadowDevice device, ProjectLab projectLab)
        {
            var connectedCount = 0;
            var completed = false;

            var wifi = device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += async (s, e) =>
            {
                Resolver.Log.Info($"Network Connected. IP: {e.IpAddress}");
            };

            if (wifi == null) return false;

            try
            {
                Resolver.Log.Info($"Connecting to invalid SSID...");
                await wifi.Connect("INVALID_SSID", "1234567890");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to connect: {ex.Message}");

                return false;
            }

            // TODO: add some IP address testing, etc.

            var timeout = 30;

            while (!completed)
            {
                await Task.Delay(1000);
                if (timeout-- <= 0) break;
            }

            // just in case multiple connects come in
            await Task.Delay(1000);

            return connectedCount == 1;
        }
    }
}