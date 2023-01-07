using Meadow.Hardware;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class WiFiAntennaSwitchingTest<T> : ITest<T>
        where T : MeadowF7TestDevice
    {
        public Task<bool> RunTest(T device)
        {
            var wifi = device.Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            // get the current antenna
            Resolver.Log.Info($"Current antenna in use: {wifi.CurrentAntenna}");

            // change to the external antenna
            Resolver.Log.Info($"Switching to external antenna");
            wifi.SetAntenna(AntennaType.External, persist: false);

            if (wifi.CurrentAntenna != AntennaType.External)
            {
                Resolver.Log.Error("Could not switch antenna to external");
                return Task.FromResult(false);
            }

            Resolver.Log.Info($"Switching to onboard antenna");
            wifi.SetAntenna(AntennaType.OnBoard, persist: false);
            if (wifi.CurrentAntenna != AntennaType.OnBoard)
            {
                Resolver.Log.Error("Could not switch antenna to onboard");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}