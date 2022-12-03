
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Validation
{
    public class WiFiConnectionInvalidPasscodeTest : ITest
    {
        public async Task<bool> RunTest(IMeadowDevice device, ProjectLab projectLab)
        {
            var completed = false;
            var success = false;

            var wifi = device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
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
                Resolver.Log.Info($"Connecting to valid SSID with invalid passcode...");
                await wifi.Connect("BOBS_YOUR_UNCLE", "BAD_PASSCODE");
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