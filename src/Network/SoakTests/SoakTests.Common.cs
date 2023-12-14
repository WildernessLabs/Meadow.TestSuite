using System;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Http;

using Meadow;
using Meadow.Devices;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;

namespace SoakTests.Common;

public static class Helpers
{
    /// <summary>
    /// Get the specified resource from the network.
    /// </summary>
    /// <param name="uri">Network resource to request.</param>
    public static async Task GetWebPageViaHttpClient(string uri)
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
    private static void ConsoleLog(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: {message}");
    }

    public static void WaitForNetworkConnection(F7FeatherBase device)
    {
        SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);

        if (device.PlatformOS.SelectedNetwork == IPlatformOS.NetworkConnectionType.Ethernet)
        {
            WaitForEthernetConnection(semaphore, device);
        }
        else
        {
            WaitForWiFiConnection(semaphore, device);
        }
        semaphore.Wait();
        ConsoleLog("Network connection established.");

        static void WaitForWiFiConnection(SemaphoreSlim semaphore, F7FeatherBase device)
        {
            ConsoleLog($"Connecting to router via WiFi.");
            var wifi = device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (!wifi.IsConnected)
            {
                wifi.NetworkConnected += (s, e) =>
                {
                    ConsoleLog("WiFi connected.");
                    ConsoleLog($"IP Address: {wifi.IpAddress}");
                    semaphore.Release();
                };
            }
            else
            {
                ConsoleLog("WiFi already connected.");
                ConsoleLog($"IP Address: {wifi.IpAddress}");
                semaphore.Release();
            }
        }

        static void WaitForEthernetConnection(SemaphoreSlim semaphore, F7FeatherBase device)
        {
            ConsoleLog($"Connecting to router via wired ethernet.");
            var ethernet = device.NetworkAdapters.Primary<IWiredNetworkAdapter>();
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
    }
}