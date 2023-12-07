using Meadow.Hardware;
using Meadow.Devices;
using Meadow.Foundation;
using System;
using System.Threading;
using FeatherF7Test.Hardware;
using FeatherF7Test.Services;
using System.Threading.Tasks;
using System.Net.Http;


using Meadow.Foundation.Leds;

namespace FeatherF7Test
{
    internal class MainController
    {
        TestSettings _config;

        F7FeatherBase _device;

        IOLEDBoardHardware _hardware;

        DisplayController _displayService;

        public MainController(F7FeatherBase device)
        {
            _device = device;
        }

        public void Initialize()
        {
            _config = new TestSettings();

            _hardware = new OLEDBoardHardware();
            _hardware.Initialize(_device);

            _displayService = new DisplayController(_hardware.Display);

            string title = "HttpGetSoakTest";

            string[] text = new string[]
            {
                "",  ""
            };

            _displayService.UpdateTitle(title);
            _displayService.UpdateText(text);
        }

        public void Run()
        {
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
                _hardware.Led1.IsOn = !_hardware.Led1.IsOn;
                counter++;
                if ((modulo == 0) || (counter % modulo == 0) || (counter < 10))
                {
                    _displayService.Log($"{counter}");
                }
                try
                {
                    GetWebPageViaHttpClient(_config.RequestUri).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    Thread.Sleep(Timeout.Infinite);
                }
                if (_config.DelayBetweenRequestsMs > 0)
                {
                    Thread.Sleep(_config.DelayBetweenRequestsMs);
                }
            }
            _displayService.Log("Done.");

            Thread.Sleep(Timeout.Infinite);
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
                    _displayService.Log("Request time out.");
                }
                catch (Exception e)
                {
                    _displayService.Log($"Request failed: {e.Message}");
                }
            }
        }
    }
}