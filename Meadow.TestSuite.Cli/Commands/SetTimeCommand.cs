using CliFx.Attributes;
using CliFx.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    [Command("time set")]
    public class SetTimeCommand : CommandBase
    {
        [CommandParameter(0)]
        public string TimeString { get; init; }

        public SetTimeCommand(DirectorProvider directorProvider)
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

            DateTime dt;

            if (string.IsNullOrEmpty(TimeString) || TimeString == "now")
            {
                dt = DateTime.Now.ToUniversalTime();
            }
            else if (DateTime.TryParse(TimeString, out dt))
            {
                // valid
            }
            else
            {
                console.Output.WriteLine($"Invalid time input '{TimeString}'");
                return;
            }

            var director = DirectorProvider.GetDirector(Transport);

            console.Output.WriteLine($"Setting worker time...");
            await director.SetTime(dt);
            console.Output.WriteLine($"Done.");
        }
    }
}