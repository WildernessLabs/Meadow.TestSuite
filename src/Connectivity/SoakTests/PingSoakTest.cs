using System;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using SoakTests.Common;

namespace SoakTests;

/// <summary>
/// Perform the async Ping soak test.
/// </summary>
class PingSoakTest : ISoakTest
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
        var ping = new Ping();

        while (true)
        {
            try
            {
                var result = ping.Send(_config.RequestUri);

                switch (result.Status)
                {
                    case IPStatus.Success:
                        Console.WriteLine($"Ping response in: {result.RoundtripTime} ms");
                        break;
                    default:
                        Console.WriteLine($"Ping failed: {result.Status}");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ping failed: {e.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    /// <summary>
    /// Perform any necessary cleanup at the end of the test run.
    /// </summary>
    public void Teardown()
    {
    }
}