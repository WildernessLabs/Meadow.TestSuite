using System;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Net;
using System.Drawing;

using Meadow;
using Meadow.Devices;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using Meadow.Units;
using System.IO;

namespace HttpGetSoakTest
{
    // public class MeadowApp : App<F7FeatherV1>, IApp
    // public class MeadowApp : App<F7FeatherV2>, IApp
    public class MeadowApp : App<F7CoreComputeV2>, IApp
    {
        /// <summary>
        /// LED to be used as a status indicator to show the main loop is still active.
        /// </summary>
        IDigitalOutputPort _led;

        /// <summary>
        /// Configuration settings for the application.
        /// </summary> 
        HttpGetSoakTestSettings _config;

        public MeadowApp()
        {

        }

        /// <summary>
        /// Perform system setup.
        /// </summary>
        /// <returns></returns>
        async Task IApp.Initialize()
        {
            //
            //  ProjLab V3
            //
            _led = Device.CreateDigitalOutputPort(Device.Pins.D11);

            //
            //  CCM ethernet board V1.
            //
            // _led = Device.CreateDigitalOutputPort(Device.Pins.D20);

            //
            //  Feather V2
            //
            // _led = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);

            _led.State = true;
            _config = new HttpGetSoakTestSettings();
            Log($"Request URI: {_config.RequestUri}");
            Log($"Number of requests: {_config.NumberOfRequests}");
            Log($"Delay between requests: {_config.DelayBetweenRequestsMs} ms");

            base.Initialize();
        }

        /// <summary>
        /// Main program loop is here.
        /// </summary>
        public override async Task Run()
        {
            //
            //  First we set up the network (either ethernet or WiFi)
            //  depending upon the configuration.  If we are using WiFi
            //  then the system must be configured to automatic connection
            //  to the default access point.
            //
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (Device.PlatformOS.SelectedNetwork == IPlatformOS.NetworkConnectionType.Ethernet)
            {
                Log("Configured for wired ethernet.");
                var ethernet = Device.NetworkAdapters.Primary<IWiredNetworkAdapter>();
                ethernet.NetworkConnected += (s, e) =>
                {
                    Log("Ethernet connected.");
                    Log($"IP Address: {ethernet.IpAddress}");
                };
            }
            else
            {
                Log("Configured for WiFi.");

                wifi.NetworkConnected += (s, e) =>
                {
                    Log("WiFi connected.");
                    Log($"IP Address: {wifi.IpAddress}");
                };
                Log($"Connecting to default access point.");

                while (!wifi.IsConnected)
                {
                    Thread.Sleep(500);
                }
            }
            //
            //  Now some variables used to show progress.
            //
            int counter = 0;
            int modulo = _config.NumberOfRequests switch
            {
                > 100 => 100,
                > 10 => 10,
                _ => 0
            };
            //
            //  Finally, we can start the main loop.
            //
            while (counter < _config.NumberOfRequests)
            {
                _led.State = !_led.State;
                counter++;
                if ((modulo == 0) || (counter % modulo == 0))
                {
                    Log($"Request: {counter}");
                }
                try
                {
                    GetWebPageViaHttpClient(_config.RequestUri).Wait();
                }
                catch (Exception ex)
                {
                    Log($"Exception: {ex.Message}");
                    Thread.Sleep(Timeout.Infinite);
                }
                if (_config.DelayBetweenRequestsMs > 0)
                {
                    Thread.Sleep(_config.DelayBetweenRequestsMs);
                }
            }
            Log("Done.");
        }

        /// <summary>
        /// Get the specified resource from the network.
        /// </summary>
        /// <param name="uri">Network resource to request.</param>
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

        /// <summary>
        /// Show the message passed in on the console with a time stamp.
        /// </summary>
        /// <param name="message">Message to be shown.</param>
        private void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: {message}");
        }
    }
}