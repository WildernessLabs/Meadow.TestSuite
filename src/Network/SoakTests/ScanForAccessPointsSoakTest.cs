using System;
using System.Threading.Tasks;
using SoakTests.Common;

using Meadow.Hardware;
using Meadow.Gateway.WiFi;

namespace SoakTests;

/// <summary>
/// Perform the async HttpClient-based soak test.
/// </summary>
class ScanForAccessPointsSoakTest : ISoakTest
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
    }

    /// <summary>
    /// Execute the test once.
    /// </summary>
    public async Task Execute()
    {
        Helpers.ConsoleLog("Getting list of access points.");
        var wifi = Helpers.DeviceUnderTest.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
        var networks = await wifi.Scan(TimeSpan.FromSeconds(60));

        if (networks.Count > 0)
        {
            Helpers.ConsoleLog("|-------------------------------------------------------------|---------|");
            Helpers.ConsoleLog("|         Network Name             | RSSI |       BSSID       | Channel |");
            Helpers.ConsoleLog("|-------------------------------------------------------------|---------|");

            foreach (WifiNetwork accessPoint in networks)
            {
                Helpers.ConsoleLog($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |");
            }
        }
        else
        {
            Helpers.ConsoleLog($"No access points detected.");
        }
    }

    /// <summary>
    /// Perform any necessary cleanup at the end of the test run.
    /// </summary>
    public void Teardown()
    {
    }
}