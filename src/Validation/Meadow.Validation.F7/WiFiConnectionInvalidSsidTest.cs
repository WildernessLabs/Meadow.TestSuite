using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class WiFiConnectionInvalidSsidTest<T> : ITest<T>
        where T : IDeviceUnderTest<IMeadowDevice>
    {
        public async Task<bool> RunTest(T device)
        {
            var completed = false;
            var success = false;

            var wifi = device.Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (wifi == null) return false;

            wifi.NetworkConnected += (s, e) =>
            {
                Resolver.Log.Info($"Network Connected. IP: {e.IpAddress}");
            };
            wifi.NetworkError += (s, a) =>
            {
                Resolver.Log.Info($"Network Error: {a.ErrorCode}");
            };

            try
            {
                Resolver.Log.Info($"Connecting to invalid SSID...");
                await wifi.Connect("INVALID_SSID", "1234567890");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to connect: {ex.Message}");

                completed = true;
            }

            success = !wifi.IsConnected;

            var timeout = 30;

            while (!completed)
            {
                await Task.Delay(1000);
                if (timeout-- <= 0) break;
            }

            // just in case multiple connects come in
            await Task.Delay(1000);

            return success;
        }
    }
}