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
        private ITestProvider Provider { get; }
        public bool RunTestsAsync { get; set; } = true;

        internal TestRunner(ITestProvider provider, string testID)
        {
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
                Output.WriteLineIf(ShowDebug, $" Creating test instance");
                var instance = test.TestConstructor.Invoke(null);

                // inject Device
                Output.WriteLineIf(ShowDebug, $" Checking Device");
                if (test.DeviceProperty != null)
                {
                    Output.WriteLineIf(ShowDebug, $" Setting Device");
                    test.DeviceProperty.SetValue(instance, this.Provider.Device);
                }

                Output.WriteLineIf(ShowDebug, $" Invoking {test?.TestMethod.Name}");

                test.TestMethod.Invoke(instance, null);

                Output.WriteLineIf(ShowDebug, $" Invoke complete");

                // if the test didn't throw, it succeeded
                Result.State = TestState.Success;
            }
            catch (TargetInvocationException tie)
            {
                Output.WriteLine($" Test Failure: {tie.InnerException.Message}");

                Result.State = TestState.Failed;
                Result.Output.Add(tie.InnerException.Message);
            }
            catch (Exception ex)
            {
                Output.WriteLineIf(ShowDebug, $" {ex.GetType().Name}: {ex.Message}");

                Result.State = TestState.Failed;
                Result.Output.Add("Unhandled exception");
                Result.Output.Add(ex.Message);
            }
            finally
            {
                sw.Stop();
                Output.WriteLineIf(ShowDebug, $" finally block");
                Result.RunTimeSeconds = sw.Elapsed.TotalSeconds;
                Result.CompletionDate = DateTime.Now.ToUniversalTime();

                ExecutionComplete?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}


