using Munit;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Meadow.TestSuite
{
    public class TestRunner
    {
        public bool ShowDebug { get; set; } = false;
        public TestResult Result { get; private set; }
        private ITestProvider Provider { get; }

        // NOTE: as of beta 4.x, exceptions in a method loaded via reflaction cause a mono crash, so we have to
        // use a static flag, which in turn means tests must be run synchronously.  When the bug is fixed, we can set these both to `true`
        public bool UseExceptionsForAssertControl { get; set; } = true;
        public bool RunTestsAsync { get; set; } = false;

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
            Assert.ThrowOnFail = UseExceptionsForAssertControl;
            Assert.LastFailMessage = null;
            Assert.HasFailed = false;

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

                if (UseExceptionsForAssertControl)
                {
                    // if the test didn't throw, it succeeded
                    Result.State = TestState.Success;
                }
                else
                {
                    if (Assert.HasFailed)
                    {
                        Result.State = TestState.Failed;
                        Result.Output.Add(Assert.LastFailMessage);
                    }
                    else
                    {
                        Result.State = TestState.Success;
                    }

                    Output.WriteLineIf(ShowDebug, $" Invoke result: {Result.State}");
                }
            }
            catch (TargetInvocationException tie)
            {
                Output.WriteLineIf(ShowDebug, $" TargetInvocationException");

                Result.State = TestState.Failed;
                Result.Output.Add(tie.InnerException.Message);
            }
            catch (TestFailedException tfe)
            {
                Output.WriteLineIf(ShowDebug, $" TestFailedException");

                Result.State = TestState.Failed;
                Result.Output.Add(tfe.Message);
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
            }
        }
    }
}


