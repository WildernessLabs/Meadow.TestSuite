using Munit;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Meadow.TestSuite
{
    public class TestRunner
    {
        public bool ShowDebug { get; set; } = true;
        public TestResult Result { get; private set; }

        private ITestProvider Provider { get; }

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

            Task.Run(() =>
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

                    Output.WriteLine($" Invoking {test?.TestMethod.Name}");

                    test.TestMethod.Invoke(instance, null);

                    Output.WriteLineIf(ShowDebug, $" Invoke complete");
                        // if the test didn't throw, it succeeded

                    Result.State = TestState.Success;
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
            });

            return Result;
        }
    }
}


