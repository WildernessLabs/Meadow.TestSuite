using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    [Command("test list")]
    public class TestListCommand : CommandBase
    {
        public TestListCommand(DirectorProvider directorProvider)
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
            var tests = await director.GetTestNames();

            if (tests.Length == 0)
            {
                console.Output.WriteLine($"  <none>");
            }
            else
            {
                foreach (var t in tests)
                {
                    console.Output.WriteLine($"  {t}");
                }
            }
        }
    }
}