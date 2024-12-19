using ProjectLabTest.Hardware;
using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

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

            return Task.CompletedTask;
        }

        public override Task Run()
        {
            mainController.Run();

            return Task.CompletedTask;
        }
    }
}