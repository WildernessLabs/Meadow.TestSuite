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
        public string? Destination { get; set; }

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
                // check to see if it's a directory
                if (Directory.Exists(Source))
                {
                    await director.SendDirectory(new DirectoryInfo(Source), Destination);
                    return;
                }

                // check to see if it is a pattern match
                var sources = Directory.GetFiles(Source);
                if (sources.Length > 0)
                {
                    foreach (var f in Directory.GetFiles(Source))
                    {
                        await director.SendFile(new FileInfo(f), Destination);
                    }
                }

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