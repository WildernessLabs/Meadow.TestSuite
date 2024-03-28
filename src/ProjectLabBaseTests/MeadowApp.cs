using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace ProjectLabBaseTests;

public class MeadowApp : App<F7CoreComputeV2>
{
    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        return base.Initialize();
    }

    public override async Task Run()
    {
        Resolver.Log.Info(">>> TEST STARTED <<<");

        Resolver.Log.Info(">>> TEST INFO First Test Running <<<");
        await Task.Delay(5000);

        Resolver.Log.Info(">>> TEST SUCCEEDED <<<");
        //            Resolver.Log.Info(">>> TEST FAILED <<<");
    }

}