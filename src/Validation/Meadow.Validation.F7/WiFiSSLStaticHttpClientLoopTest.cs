using Meadow.Hardware;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Meadow.Validation
{
    public class WiFiSSLStaticHttpClientLoopTest<T> : ITest<T>
        where T : MeadowTestDevice
    {
        private const string BaseUrl = "https://postman-echo.com";
        private const string Payload = "payload";
        private static HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromMinutes(5)
        };
        private static Stopwatch sw = new Stopwatch();

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

            for (int i = 0; i < 300; i++)
            {
                await PostStaticHttpRequestAsync(i);
            }

            return success;
        }

        public async Task PostStaticHttpRequestAsync(int count)
        {
            Console.WriteLine($"Requesting {_httpClient.BaseAddress} using static HTTP client - {DateTime.Now} #{count} ");
            
            try
            {
                sw.Start();
                using (var content = new StringContent(Payload, Encoding.UTF8))
                using (var result = await _httpClient.PostAsync("post", content).ConfigureAwait(false))
                {
                    sw.Stop();
                    Console.WriteLine($"Request {(result.IsSuccessStatusCode ? "succeeded" : "failed")} in {sw.Elapsed.TotalMilliseconds} ms");
                    sw.Reset();
                }
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
