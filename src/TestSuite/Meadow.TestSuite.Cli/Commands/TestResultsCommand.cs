using CliFx.Attributes;
using CliFx.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    [Command("test results")]
    public class TestResultsCommand : CommandBase
    {
        [CommandOption("name", 'n', Description = "Test Name")]
        public string TestName { get; init; }

        [CommandOption("id", 'i', Description = "Test ID")]
        public Guid TestID { get; init; }

        public TestResultsCommand(DirectorProvider directorProvider)
            : base(directorProvider)
        {
        }

        public override async ValueTask ExecuteAsync(IConsole console)
        {
            if (Transport == null)
            {
                console.Output.WriteLine($"A transport is required");
                return;
            }

            var director = DirectorProvider.GetDirector(Transport);

            if (TestID != Guid.Empty)
            {
                if (!string.IsNullOrEmpty(TestName))
                {
                    console.Output.WriteLine($"Provide a Name or ID, not both");
                    return;
                }
                else
                {
                    console.Output.WriteLine($"Getting result...");

                    var result = await director.GetTestResults(TestID);

                    if (result == null)
                    {
                        console.Output.WriteLine($"No result for ID '{TestID}':");
                        return;
                    }

                    console.Output.WriteLine($"Result for Test ID '{TestID}':");
                    console.Output.WriteLine($"  State:    {result.State}");
                    console.Output.WriteLine($"  Run Time: {result.RunTimeSeconds:0.##} s");
                    if (result.Output?.Count > 0)
                    {
                        console.Output.WriteLine($"  Output: {result.Output}");
                    }
                }
            }
            else
            {
                console.Output.WriteLine($"Getting results...");

                TestResult[] results = null;

                if (!string.IsNullOrEmpty(TestName))
                {
                    results = await director.GetTestResults(TestName);
                }
                else
                {
                    results = await director.GetTestResults();
                }

                if (results == null || results.Length == 0)
                {
                    console.Output.WriteLine($"No results found.");
                }
                else
                {
                    console.Output.WriteLine($"Results");
                    console.Output.WriteLine($"Completed  | State    | Run Time | ID ");

                    foreach (var result in results.OrderByDescending(r => r.CompletionDate))
                    {
                        console.Output.WriteLine($"{result.CompletionDate:HH:mm:ss}   | {result.State,-8} | {string.Format("{0:0.##}", result.RunTimeSeconds),-8} | {result.ResultID} ");
                    }
                }
            }
        }
    }
}