using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    [Command("time")]
    public class GetTimeCommand : CommandBase
    {
        public GetTimeCommand(DirectorProvider directorProvider)
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

            console.Output.WriteLine($"Getting worker time...");
            var time = await director.GetTime();

            console.Output.WriteLine($"{time:MM/dd/yy HH:mm:ss}");
        }
    }
}