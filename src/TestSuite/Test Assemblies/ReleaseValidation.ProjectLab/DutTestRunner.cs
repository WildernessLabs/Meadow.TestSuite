using Meadow;
using Meadow.TestSuite;
using System;
using System.Reflection;

namespace ReleaseValidation.ProjectLab;

public class DutTestRunner
{
    private readonly IMeadowDevice _device;

    public DutTestRunner(IMeadowDevice device)
    {
        _device = device;
    }

    public TestResult ExecuteTest(TestInfo test)
    {
        var result = new TestResult();

        try
        {
            Resolver.Log.Debug($" Creating test instance");
            var instance = test.TestConstructor.Invoke(null);

            // inject Device
            if (test.DeviceProperty != null)
            {
                Resolver.Log.Debug($" Injecting the Device property");
                test.DeviceProperty.SetValue(instance, _device);
            }

            result.StartedTimestamp = DateTime.UtcNow;
            test.TestMethod.Invoke(instance, null);

            // if the test didn't throw, it succeeded
            result.State = TestState.Success;
        }
        catch (TargetInvocationException tie)
        {
            Resolver.Log.Error($" Test Failure: {tie.InnerException.Message}");

            result.State = TestState.Failed;
            result.Output.Add(tie.InnerException.Message);
        }
        catch (Exception ex)
        {
            Resolver.Log.Debug($" {ex.GetType().Name}: {ex.Message}");

            result.State = TestState.Failed;
            result.Output.Add("Unhandled exception");
            result.Output.Add(ex.Message);
        }
        finally
        {
            result.CompletedTimestamp = DateTime.UtcNow;
        }

        return result;
    }
}
