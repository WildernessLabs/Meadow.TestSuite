using System;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http;

using Meadow;
using Meadow.Devices;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;

namespace SoakTests.Common;

/// <summary>
/// Helper class for the tests.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Device under test.
    /// </summary>
    /// <remarks>
    /// This property is from the main application class and is set to enable the <i>>Device</i> object
    /// to be accessed from test classes.
    /// </remarks>
    /// <value>Device object from the main application.</value>
    public static IMeadowDevice DeviceUnderTest { get; set; }

    /// <summary>
    /// Delegate for displaying log messages on the display.
    /// </summary>
    /// <param name="message"></param>
    public delegate void DisplayLogHandler(string message);

    /// <summary>
    /// Display log messages on the display.
    /// </summary>
    public static DisplayLogHandler DisplayLogMessage;

    /// <summary>
    /// Get the specified resource from the network.
    /// </summary>
    /// <param name="uri">Network resource to request.</param>
    public static async Task GetWebPageViaHttpClient(string uri)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                ConsoleLog($"Starting request to {uri}...");
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                ConsoleLog("Request completed successfully.");
            }
            catch (TaskCanceledException)
            {
                ConsoleLog("Request time out.");
            }
            catch (Exception e)
            {
                ConsoleLog($"Request failed: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Show the message passed in on the console with a time stamp.
    /// </summary>
    /// <param name="message">Message to be shown.</param>
    public static void ConsoleLog(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: {message}");
    }

    /// <summary>
    /// Wait for a network connection to be established.
    /// </summary>
    public static void WaitForNetworkConnection()
    {
        SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);

        if (DeviceUnderTest.PlatformOS.SelectedNetwork == IPlatformOS.NetworkConnectionType.Ethernet)
        {
            WaitForEthernetConnection(semaphore);
        }
        else
        {
            WaitForWiFiConnection(semaphore);
        }
        semaphore.Wait();
        ConsoleLog("Network connection established.");

        #region ----------- Internal Methods (WaitForNetworkConnection) ------------

        /// <summary>
        /// Wait for a WiFi connection.
        /// </summary>
        /// <param name="semaphore">Semaphore used by the caller to indicate that a connection has been established.</param>
        static void WaitForWiFiConnection(SemaphoreSlim semaphore)
        {
            ConsoleLog($"Connecting to router via WiFi.");
            var wifi = DeviceUnderTest.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (wifi.IsConnected)
            {
                ConsoleLog("WiFi already connected.");
                ConsoleLog($"IP Address: {wifi.IpAddress}");
                semaphore.Release();
            }
            else
            {
                wifi.NetworkConnected += (s, e) =>
                {
                    ConsoleLog("WiFi connected.");
                    ConsoleLog($"IP Address: {wifi.IpAddress}");
                    semaphore.Release();
                };
            }
        }

        /// <summary>
        /// Wait for an ethernet connection.
        /// </summary>
        /// <param name="semaphore">Semaphore used by the caller to indicate that a connection has been established.</param>
        static void WaitForEthernetConnection(SemaphoreSlim semaphore)
        {
            ConsoleLog($"Connecting to router via wired ethernet.");
            var ethernet = DeviceUnderTest.NetworkAdapters.Primary<IWiredNetworkAdapter>();
            if (ethernet.IsConnected)
            {
                ConsoleLog("Ethernet already connected.");
                ConsoleLog($"IP Address: {ethernet.IpAddress}");
                semaphore.Release();
            }
            else
            {
                ethernet.NetworkConnected += (s, e) =>
                {
                    ConsoleLog("Ethernet connected.");
                    ConsoleLog($"IP Address: {ethernet.IpAddress}");
                    semaphore.Release();
                };
            }
        }
        #endregion ---------- Internal Methods (WaitForNetworkConnection) ----------
    }

    /// <summary>
    /// Convert a URL into an endpoint.
    /// </summary>
    /// <param name="url">URL of interest.</param>
    /// <returns>IPEndpoint</returns>
    public static IPEndPoint GetEndpoint(string url)
    {
        var uri = new Uri(url, UriKind.Absolute);

        IPAddress ipAddress = null;
        if (!IPAddress.TryParse(uri.Host, out ipAddress))
        {
            var ipHostInfo = Dns.GetHostEntry(uri.DnsSafeHost);
            ipAddress = ipHostInfo.AddressList[0];
        }
        return new(ipAddress, uri.Port);
    }
}