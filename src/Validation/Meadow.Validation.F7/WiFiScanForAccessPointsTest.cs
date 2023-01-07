using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System.Threading.Tasks;
using System;
using Meadow.Gateways.Bluetooth;

namespace Meadow.Validation
{
    public class WiFiScanForAccessPointsTest<T> : ITest<T>
        where T : MeadowF7TestDevice
    {
        public async Task<bool> RunTest(T device)
        {
            Console.WriteLine("Scanning for access points...");

            var wifi = device.Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            await ScanForAccessPoints(wifi);

            return true;
        }

        async Task ScanForAccessPoints(IWiFiNetworkAdapter wifi)
        {
            Console.WriteLine("Getting list of access points");
            var networks = await wifi.Scan(TimeSpan.FromSeconds(60));

            if (networks.Count > 0)
            {
                Console.WriteLine("|-------------------------------------------------------------|---------|");
                Console.WriteLine("|         Network Name             | RSSI |       BSSID       | Channel |");
                Console.WriteLine("|-------------------------------------------------------------|---------|");

                foreach (WifiNetwork accessPoint in networks)
                {
                    Console.WriteLine($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |");
                }
            }
            else
            {
                Console.WriteLine($"No access points detected");
            }
        }
    }
}
