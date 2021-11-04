using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    [Command("test run")]
    public class TestRunCommand : CommandBase
    {
        [CommandParameter(0)]
        public string TestName { get; init; }

        public TestRunCommand(DirectorProvider directorProvider)
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

            console.Output.WriteLine($"List of Tests:");
            var result = await director.ExecuteTest(TestName);

            console.Output.WriteLine($"Running '{TestName}' as Result ID {result.ResultID}");
        }
    }
}