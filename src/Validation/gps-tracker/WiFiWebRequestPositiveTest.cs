
using Meadow;
using Meadow.Hardware;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Validation
{
    public class WiFiWebRequestPositiveTest : ITest
    {
        public async Task<bool> RunTest(IF7CoreComputeMeadowDevice device)
        {
            var success = true;

            if (Resolver.Device.PlatformOS.SelectedNetwork != IPlatformOS.NetworkConnectionType.WiFi)
            {
                Resolver.Log.Error($"WiFi is not the selected network adapter. {Resolver.Device.PlatformOS.SelectedNetwork} is selected.");
                return false;
            }

            var wifi = device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (wifi == null) return false;

            if (wifi.IsConnected)
            {
                await wifi.Disconnect(false);
            }

            try
            {
                Resolver.Log.Info($"Connecting to valid AP");
                await wifi.Connect("BOBS_YOUR_UNCLE", "1234567890", TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to connect: {ex.Message}");
                return false;
            }

            var timeout = 30;

            while (!wifi.IsConnected)
            {
                await Task.Delay(1000);
                if (timeout-- <= 0) break;
            }

            Resolver.Log.Info($"Making web request...");
            try
            {
                var sw = Stopwatch.StartNew();

                var client = new HttpClient();
                var responseText = await client.GetStringAsync("http://www.asciichart.com");
                sw.Stop();
                var lines = responseText.Where(c => c == '\n').Count();
                Resolver.Log.Info($"Web request returned {lines} lines and took {sw.Elapsed.TotalSeconds} seconds");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Request failed: {ex.Message}");
                return false;
            }

            return success;
        }
    }
}