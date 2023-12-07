using ProjectLabTest.Hardware;
using ProjectLabTest.Services;
using Meadow.Foundation.Leds;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace ProjectLabTest
{
    internal class MainController
    {
        TestSettings _config;

        IProjectLabV3Hardware _hardware;

        DisplayController _displayService;

        public MainController(IProjectLabV3Hardware hardware)
        {
            this._hardware = hardware;
        }

        public void Initialize()
        {
            _config = new TestSettings();

            _hardware.Initialize();

            _displayService = new DisplayController(_hardware.Display);

            string title = "HttpGetSoakTest";

            string[] text = new string[]
            {
                "",  "", "", "", "", "", "", "", "", "", "", "", "", ""
            };

            _hardware.RgbPwmLed.IsOn = false;
            _hardware.RgbPwmLed.SetColor(Meadow.Foundation.Color.Blue);

            _displayService.UpdateTitle(title);
            _displayService.UpdateText(text);

            _displayService.Log($"Request URI: {_config.RequestUri}");
            _displayService.Log($"Number of requests: {_config.NumberOfRequests}");
            _displayService.Log($"Delay between requests: {_config.DelayBetweenRequestsMs} ms");
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
                _hardware.RgbPwmLed.IsOn = !_hardware.RgbPwmLed.IsOn;
                counter++;
                if ((modulo == 0) || (counter % modulo == 0))
                {
                    _displayService.Log($"Request: {counter}");
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