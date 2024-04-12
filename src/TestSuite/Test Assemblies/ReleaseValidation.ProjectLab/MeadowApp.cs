using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;

namespace ReleaseValidation.ProjectLab;

public class MeadowApp : App<F7CoreComputeV2>
{
    private InProcTestProvider _provider;
    private DutTestRunner _testRunner;

    public override Task OnError(Exception e)
    {
        Resolver.Log.Info($">>> {e.Message} <<<");
        Resolver.Log.Info(">>> CRASH <<<");

        return base.OnError(e);
    }

    public override Task Initialize()
    {
        _testRunner = new DutTestRunner(Device);
        _provider = new InProcTestProvider();

        _provider.Load();

        return base.Initialize();
    }

    public override async Task Run()
    {
        Resolver.Log.Info(">>> START TEST SET <<<");
        Resolver.Log.Info(">>> HARDWARE: ProjLab 3e <<<");
        Resolver.Log.Info($">>> OS: {Device.Information.OSVersion} <<< ");
        Resolver.Log.Info($">>> CORE: {typeof(MeadowOS).Assembly.GetName().Version} <<< ");

        foreach (var test in _provider.GetTests())
        {
            try
            {
                Resolver.Log.Info($">>> BEGIN: {test.TestName} <<<");

                var result = _testRunner.ExecuteTest(test);
                if (result.State == Meadow.TestSuite.TestState.Success)
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
                Resolver.Log.Info($">>> {test.TestName} threw unhandled exception: {e.Message} <<<");
                Resolver.Log.Info(">>> FAIL <<<");
            }
        }

        Resolver.Log.Info(">>> END TEST SET <<<");
    }

}