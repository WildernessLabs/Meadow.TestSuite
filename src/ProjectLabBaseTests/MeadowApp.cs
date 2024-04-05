using Meadow;
using Meadow.Devices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectLabBaseTests;

public class MeadowApp : App<F7CoreComputeV2>
{
    private List<IDeviceTest> _tests = new();

    public override Task OnError(Exception e)
    {
        Resolver.Log.Info($">>> {e.Message} <<<");
        Resolver.Log.Info(">>> CRASH <<<");

        return base.OnError(e);
    }

    public override Task Initialize()
    {
        // don't use reflection (for now) to prevent linker shenanigans
        _tests.Add(new COM1LoopbackTest());
        _tests.Add(new PWMToAnalogTest());

        return base.Initialize();
    }

    public override async Task Run()
    {
        Resolver.Log.Info(">>> START TEST SET <<<");
        Resolver.Log.Info(">>> HARDWARE: ProjLab 3e <<<");
        Resolver.Log.Info($">>> OS: {Device.Information.OSVersion} <<< ");
        Resolver.Log.Info($">>> CORE: {typeof(MeadowOS).Assembly.GetName().Version} <<< ");

        foreach (var test in _tests)
        {
            try
            {
                Resolver.Log.Info($">>> BEGIN: {test.Name} <<<");

                if (test.Execute(Device))
                {
                    Resolver.Log.Info(">>> SUCCESS <<<");
                }
                else
                {
                    Resolver.Log.Info(">>> FAIL <<<");
                }
            }
            catch (Exception e)
            {
                Resolver.Log.Info($">>> {test.Name} threw unhandled exception: {e.Message} <<<");
                Resolver.Log.Info(">>> FAIL <<<");
            }
        }

        Resolver.Log.Info(">>> END TEST SET <<<");
    }

}