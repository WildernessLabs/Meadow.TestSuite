using Meadow;
using Meadow.Gateways.Bluetooth;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;
using Validation;

namespace F7Feather.Tests
{
    internal class Antenna_Switiching : ITestFeatherF7
    {
        public async Task<bool> RunTest(IF7MeadowDevice device)
        {
            var wifi = device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            // get the current antenna
            Resolver.Log.Info($"Current antenna in use: {wifi.CurrentAntenna}");

            // change to the external antenna
            Resolver.Log.Info($"Switching to external antenna");
            wifi.SetAntenna(AntennaType.External, persist: false);

            if(wifi.CurrentAntenna != AntennaType.External)
            {
                Resolver.Log.Error("Could not switch antenna to external");
                return false;
            }

            Resolver.Log.Info($"Switching to onboard antenna");
            wifi.SetAntenna(AntennaType.OnBoard, persist: false);
            if (wifi.CurrentAntenna != AntennaType.OnBoard)
            {
                Resolver.Log.Error("Could not switch antenna to onboard");
                return false;
            }

            return true;
        }
    }
}