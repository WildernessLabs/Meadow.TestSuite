using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;

namespace ProjectLabBaseTests;

public class MeadowApp : App<F7CoreComputeV2>
{
    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        return base.Initialize();
    }

    public override Task OnError(Exception e)
    {
        Resolver.Log.Info($">>> {e.Message} <<<");
        Resolver.Log.Info(">>> TEST FAILED <<<");

        return base.OnError(e);
    }

    public override async Task Run()
    {
        Resolver.Log.Info(">>> START TEST SET <<<");

        Resolver.Log.Info(">>> BEGIN: TEST A <<<");

        Resolver.Log.Info(">>> First Test Running <<<");
        await Task.Delay(5000);

        Resolver.Log.Info(">>> SUCCESS <<<");



        Resolver.Log.Info(">>> BEGIN: TEST B <<<");

        Resolver.Log.Info(">>> Second Started Running <<<");
        await Task.Delay(5000);
        Resolver.Log.Info(">>> Second Finished with error: foo <<<");

        Resolver.Log.Info(">>> FAIL <<<");

        Resolver.Log.Info(">>> END TEST SET <<<");
    }

}