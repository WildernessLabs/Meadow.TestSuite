using Meadow.Hardware;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class WiFiConnectionPositiveTest<T> : ITest<T>
        where T : IDeviceUnderTest<IMeadowDevice>
    {
        public async Task<bool> RunTest(T device)
        {
            var connectedCount = 0;
            var disconnectedCount = 0;
            var completed = false;
            var success = true;

            if (Resolver.Device.PlatformOS.SelectedNetwork != IPlatformOS.NetworkConnectionType.WiFi)
            {
                Resolver.Log.Error($"WiFi is not the selected network adapter. {Resolver.Device.PlatformOS.SelectedNetwork} is selected.");
                return false;
            }

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

            // TODO: add some IP address testing, etc.

            // just in case multiple connects come in
            await Task.Delay(1000);

            timeout = 30;
            completed = false;

            if (wifi.IsConnected)
            {
                try
                {
                    Resolver.Log.Info($"Disconnecting from AP");

                    await wifi.Disconnect(false);

                    Resolver.Log.Info($"Disconnected from AP");
                    completed = true;
                }
                catch (Exception ex)
                {
                    Resolver.Log.Error($"Failed to disconnect: {ex.Message}");
                    success = false;
                    completed = true;
                }

                while (!completed)
                {
                    await Task.Delay(1000);
                    if (timeout-- <= 0) break;
                }
            }

            success &= connectedCount == 1;
            return success;
        }
    }
}