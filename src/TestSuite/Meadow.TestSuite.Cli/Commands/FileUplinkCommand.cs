using CliFx.Attributes;
using CliFx.Infrastructure;
using System.IO;
using System.Threading.Tasks;

namespace Meadow.TestSuite.Cli
{
    [Command("file send")]
    public class FileUplinkCommand : CommandBase
    {
        public FileUplinkCommand(DirectorProvider directorProvider)
            : base(directorProvider)
        {
        }

        [CommandOption("source", 's', Description = "Local source file", IsRequired = true)]
        public string Source { get; set; }
        [CommandOption("dest", 'd', Description = "Optional destination file name")]
        public string Destination { get; set; }

        public override async ValueTask ExecuteAsync(IConsole console)
        {
            if (Transport == null)
            {
                console.Output.WriteLine($"A transport is required");
                return;
            }

            var director = DirectorProvider.GetDirector(Transport);

            var fi = new FileInfo(Source);
            if (!fi.Exists)
            {
                console.Output.WriteLine($"Source file '{Source}' not found");
                return;
            }

            if (Destination != null)
            {
                console.Output.WriteLine($"Sending to {Destination}...");
            }
            else
            {
                console.Output.WriteLine($"Sending {fi.Name}...");
            }

            await director.SendFile(fi, Destination);

            console.Output.WriteLine($"File sent");
        }
    }
}