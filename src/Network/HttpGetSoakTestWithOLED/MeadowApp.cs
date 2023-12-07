using FeatherF7Test.Hardware;
using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace FeatherF7Test
{
    // public class MeadowApp : App<F7FeatherV1>
    public class MeadowApp : App<F7FeatherV2>
    {
        MainController mainController;

        public override Task Initialize()
        {
            mainController = new MainController(Device);
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