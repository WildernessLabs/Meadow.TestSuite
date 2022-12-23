using Meadow.Hardware;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class WiFiAntennaSwitchingTest<T> : ITest<T>
        where T : IDeviceUnderTest, IF7MeadowDevice
    {
        public async Task<bool> RunTest(T device)
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