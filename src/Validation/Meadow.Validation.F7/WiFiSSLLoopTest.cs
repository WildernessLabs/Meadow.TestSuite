using Meadow.Hardware;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class WiFiSSLLoopTest<T> : ITest<T>
        where T : MeadowTestDevice
    {
        public async Task<bool> RunTest(T device)
        {
            string SSID = "TELUSDC1E"; // "BunnyMesh";
            string PASSWORD = "tnrXFa6MVqAU"; //  "zxpvi29wt8";

            var completed = false;
            var success = false;

            var wifi = (device as MeadowF7TestDevice).Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
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
                Resolver.Log.Info($"Connecting to valid SSID with valid passcode...");

                await wifi.Connect(SSID, PASSWORD);
                completed = true;
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to connect: {ex.Message}");

                completed = true;
            }

            success = wifi.IsConnected;

            var timeout = 30;

            while (!completed)
            {
                await Task.Delay(1000);
                if (timeout-- <= 0) break;
            }

            // just in case multiple connects come in
            await Task.Delay(1000);

            // Avoid running the requests if wi-fi never connected.
            if (!wifi.IsConnected)
            {
                Resolver.Log.Error("Wi-Fi not connected");
                return false;
            }

            for (int i = 0; i < 100; i++)
            {
                await GetWebPageViaHttpClient("https://postman-echo.com/get?foo1=bar1&foo2=bar2", i);
            }

            return success;
        }

        public async Task GetWebPageViaHttpClient(string uri, int count)
        {
            Console.WriteLine($"Requesting {uri} - {DateTime.Now} #{count}");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 5, 0);

                HttpResponseMessage response = await client.GetAsync(uri);

                try
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request time out");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }
    }
}
