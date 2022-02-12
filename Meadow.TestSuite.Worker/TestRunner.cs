using Meadow.Logging;
using Munit;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Meadow.TestSuite
{
    public class TestRunner
    {
        public event EventHandler ExecutionComplete = delegate { };

        public bool ShowDebug { get; set; } = false;
        public TestResult Result { get; private set; }
        public bool RunTestsAsync { get; set; } = true;
        private ITestProvider Provider { get; }
        private ILogger? Logger { get; }

        internal TestRunner(ITestProvider provider, string testID, ILogger? logger = null)
        {
            Logger = logger;
            Provider = provider;
            Result = new TestResult(testID);
            Result.ResultID = Guid.NewGuid();
        }

        public TestResult Begin()
        {
            var test = Provider.GetTest(Result.TestID);
            if (test == null)
            {
                Result.State = TestState.Failed;
                Result.Output.Add($"Unknown test ID");
                return Result;
            }

            Result.State = TestState.Running;

            if (RunTestsAsync)
            {
                Task.Run(() =>
                {
                    ExecuteTest(test);
                });
            }
            else
            {
                ExecuteTest(test);
            }

            return Result;
        }

        private void ExecuteTest(TestInfo test)
        {
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                Logger?.Debug($" Creating test instance");
                var instance = test.TestConstructor.Invoke(null);

                // inject Device
                if (instance is MeadowTestBase { } mt)
                {
                    Logger?.Debug($" Test class is a MeadowTestBase");
                    mt.Device = this.Provider.Device;
                    mt.Logger = this.Logger;
                }
                else
                {
                    Logger?.Debug($"Test class is not a MeadowTestBase. Checking for Device property");
                    if (test.DeviceProperty != null)
                    {
                        Logger?.Debug($" Setting Device");
                        test.DeviceProperty.SetValue(instance, this.Provider.Device);
                    }
                }

                Logger?.Debug($" Invoking {test?.TestMethod.Name}");

                test.TestMethod.Invoke(instance, null);

                Logger?.Debug($" Invoke complete");

                // if the test didn't throw, it succeeded
                Result.State = TestState.Success;
            }
            catch (TargetInvocationException tie)
            {
                Logger.Error($" Test Failure: {tie.InnerException.Message}");

                Result.State = TestState.Failed;
                Result.Output.Add(tie.InnerException.Message);
            }
            catch (Exception ex)
            {
                Logger?.Debug($" {ex.GetType().Name}: {ex.Message}");

                Result.State = TestState.Failed;
                Result.Output.Add("Unhandled exception");
                Result.Output.Add(ex.Message);
            }
            finally
            {
                sw.Stop();
                Logger?.Debug($" finally block");
                Result.RunTimeSeconds = sw.Elapsed.TotalSeconds;
                Result.CompletionDate = DateTime.Now.ToUniversalTime();

                ExecutionComplete?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}


