using Meadow.Hardware;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class WiFiScanPositiveTest<T> : ITest<T>
        where T : IDeviceUnderTest
    {
        public async Task<bool> RunTest(T device)
        {
            var connectedCount = 0;
            var disconnectedCount = 0;
            var completed = false;
            var success = true;

            var wifi = device.Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (wifi == null) return false;

            wifi.NetworkConnected += (s, e) =>
            {
                connectedCount++;
                Resolver.Log.Info($"Network Connected. IP from event: {e.IpAddress} IP from adapter: {wifi.IpAddress}");

                if (e.IpAddress.Equals(IPAddress.None))
                {
                    Resolver.Log.Error($"event data IP was bad");
                    success = false;
                }

                if (wifi.IpAddress.Equals(IPAddress.None))
                {
                    Resolver.Log.Error($"adapter IP was bad");
                    success = false;
                }

                if (!e.IpAddress.Equals(wifi.IpAddress))
                {
                    Resolver.Log.Error($"adapter IP does not match event IP");
                    success = false;
                }

                completed = true;
            };

            wifi.NetworkDisconnected += (s) =>
            {
                disconnectedCount++;
                Resolver.Log.Info($"Network Disconnected. IP from adapter: {wifi.IpAddress}");
            };

            try
            {
                Resolver.Log.Info($"Connecting to valid AP");
                await wifi.Connect("BOBS_YOUR_UNCLE", "1234567890", TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to connect: {ex.Message}");
                success = false;
                completed = true;
            }

            var timeout = 30;

            while (!completed)
            {
                await Task.Delay(1000);
                if (timeout-- <= 0) break;
            }

            Resolver.Log.Info($"Scanning for Networks...");
            var scanResult = await wifi.Scan(TimeSpan.FromSeconds(30));
            Resolver.Log.Info($"{scanResult.Count} Networks found");

            if (scanResult.Count == 0)
            {
                // likely a failure
                success = false;
            }

            success &= connectedCount == 1;
            return success;
        }
    }
}