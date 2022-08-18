using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.TestSuite
{
    public class ExecuteTestsCommand : TestCommand
    {
        public string[] TestNames { get; set; }

        public ExecuteTestsCommand(IEnumerable<string> testNames)
            : this()
        {
            TestNames = testNames.ToArray();
        }

        public ExecuteTestsCommand()
        {
            CommandType = CommandType.ExecuteTests;
        }

        public override void Execute(IWorker worker)
        {
            if (TestNames == null)
            {
                Result = "TestNames is null";
            }
            else if (TestNames.Length == 0)
            {
                Result = "TestNames is empty";
            }
            else
            {
                var list = new List<TestResult>();

                var successCount = 0;
                var failCount = 0;

                worker.IndicateState(TestState.NotRun);

                foreach (var t in TestNames)
                {
                    // allow for wildcard test execution
                    if (t.Contains('*'))
                    {
                        var names = worker.Registry.GetMatchingNames(t);
                        if (names == null || names.Length == 0)
                        {
                            Console.WriteLine($"No tests found matching {t}");
                        }
                        else
                        {
                            foreach (var n in names)
                            {
                                Console.WriteLine($"Running {n}...");
                                var result = worker.ExecuteTest(n);
                                list.Add(result);
                                switch(result.State)
                                {
                                    case TestState.Success:
                                        successCount++;
                                        break;
                                    case TestState.Failed:
                                        failCount++;
                                        break;
                                }
                                Console.WriteLine($"{result.State}");
                            }

                            Console.WriteLine($"Done: {successCount} succeeded. {failCount} failed.");
                            worker.IndicateState(failCount == 0 ? TestState.Success : TestState.Failed);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Running {t}...");
                        var result = worker.ExecuteTest(t);
                        list.Add(result);
                        Console.WriteLine($"{result.State}");
                        worker.IndicateState(result.State);
                    }
                }

                Result = list;
            }
        }
    }
}