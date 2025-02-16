using System;
using System.Threading.Tasks;
using System.Diagnostics;
using SoakTests.Common;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http;

namespace SoakTests;

/// <summary>
/// Perform the async HttpClient-based soak test.
/// </summary>
class LargeFileTest : ISoakTest
{
    /// <summary>
    /// Soak test configuration object.
    /// </summary>
    SoakTestSettings _config;

    /// <summary>
    /// Setup the test.
    /// </summary>
    /// <param name="config">General soak test configuration</param>
    public void Initialize(SoakTestSettings config)
    {
        _config = config;

        Helpers.WaitForNetworkConnection();
    }

    /// <summary>
    /// Execute the test once.
    /// </summary>
    public async Task Execute()
    {
        Stopwatch stopWatch = new Stopwatch();
        using (HttpClient client = new HttpClient())
        {
            try
            {
                Helpers.ConsoleLog($"Starting request to {_config.RequestUri}...");
                Helpers.DisplayLogMessage($"Starting request:");
                Helpers.DisplayLogMessage(_config.RequestUri);

                stopWatch.Start();
                HttpResponseMessage response = await client.GetAsync(_config.RequestUri);
                stopWatch.Stop();

                response.EnsureSuccessStatusCode();
                Helpers.ConsoleLog($"Received {response.Content.Headers.ContentLength} bytes.");
                Helpers.DisplayLogMessage($"Received {response.Content.Headers.ContentLength:N0} bytes.");

                TimeSpan elapsed = stopWatch.Elapsed;
                int numberOfSeconds = (int) ((elapsed.Minutes * 60) + elapsed.Seconds);
                string time = $"{numberOfSeconds}.{elapsed.Milliseconds} seconds.";
                
                Helpers.ConsoleLog($"Request to {_config.RequestUri} completed in {time}");
                Helpers.DisplayLogMessage($"Duration: {time}");
            }
            catch (TaskCanceledException)
            {
                Helpers.ConsoleLog("Request time out.");
            }
            catch (Exception e)
            {
                Helpers.ConsoleLog($"Request failed: {e.Message}");
            }
        }
        Helpers.ConsoleLog($"Test complete");
    }

    /// <summary>
    /// Perform any necessary cleanup at the end of the test run.
    /// </summary>
    public void Teardown()
    {
    }
}