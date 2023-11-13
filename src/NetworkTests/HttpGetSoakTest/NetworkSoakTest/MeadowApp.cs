using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkHttpGetSoakTest
{
    // public class MeadowApp : App<F7FeatherV1>, IApp
    // public class MeadowApp : App<F7FeatherV2>, IApp
    public class MeadowApp : App<F7CoreComputeV2>, IApp
    {
        private static Semaphore semaphore = new Semaphore(0, 1);
        private NetworkHttpGetSoakTestConfiguration config = new NetworkHttpGetSoakTestConfiguration();

        private IDigitalOutputPort _red;
        private IDigitalOutputPort _green;
        private IDigitalOutputPort _blue;

        public override Task Initialize()
        {
            //
            //  Feather V2 - use false for off, true for on
            //
            // _red = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            // _green = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);
            // _blue = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);

            //
            //  CoreCompute Debug Board V2.a, use false for on, true for off
            //
            _red = Device.CreateDigitalOutputPort(Device.Pins.D09);
            _red.State = true;
            _green = Device.CreateDigitalOutputPort(Device.Pins.D10);
            _green.State = true;
            _blue = Device.CreateDigitalOutputPort(Device.Pins.D11);

            return base.Initialize();
        }

        public override async Task Run()
        {
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += WiFiAdapter_NetworkConnected;

            Log("Connecting to WiFi...");
            semaphore.WaitOne();

            Log("Starting soak test...");
            Log($"Using URL: {config.TestURL}, for {config.Iterations:n0} iterations.");
            uint count = 0;
            while (count < config.Iterations)
            {
                await GetWebPageViaHttpClient(config.TestURL);
                _blue.State = !_blue.State;

                count++;
                if ((count < 50) || (count % 100 == 0))
                {
                    Log($"Request count - {count:n0}");
                    Log($"Free memory: {GC.GetTotalMemory(true):n0} bytes");
                }
            }
            Log($"Test complete, {config.Iterations:n0} iterations.");
            _blue.State = false;
            _green.State = true;
        }

        void WiFiAdapter_NetworkConnected(INetworkAdapter networkAdapter, NetworkConnectionEventArgs e)
        {
            Log("Connection request completed");
            semaphore.Release();
        }

        private void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: {message}");
        }

        public async Task GetWebPageViaHttpClient(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(uri);

                try
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                }
                catch (TaskCanceledException)
                {
                    Log("Request time out.");
                }
                catch (Exception e)
                {
                    Log($"Request failed: {e.Message}");
                }
            }
        }
    }
}
