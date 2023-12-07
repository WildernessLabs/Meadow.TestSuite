using ProjectLabTest.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Threading.Tasks;
using System.Threading;

namespace ProjectLabTest
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainController mainController;

        public override Task Initialize()
        {
            var hardware = new ProjectLabV3Hardware();
            mainController = new MainController(hardware);
            mainController.Initialize();

            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (Device.PlatformOS.SelectedNetwork == IPlatformOS.NetworkConnectionType.Ethernet)
            {
                var ethernet = Device.NetworkAdapters.Primary<IWiredNetworkAdapter>();
            }
            else
            {
                while (!wifi.IsConnected)
                {
                    Thread.Sleep(500);
                }
            }
            
            return Task.CompletedTask;
        }

        public override Task Run()
        {
            mainController.Run();

            return Task.CompletedTask;
        }
    }
}