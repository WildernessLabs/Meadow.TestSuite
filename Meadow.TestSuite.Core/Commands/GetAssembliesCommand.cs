using System;
using System.IO;
using System.Linq;

namespace Meadow.TestSuite
{
    public class GetAssembliesCommand : TestCommand
    {
        public GetAssembliesCommand()
        {
            CommandType = CommandType.EnumerateAssemblies;
        }

        public string Folder { get; set; }

        public override void Execute()
        {
            Console.WriteLine($"Getting assemblies in {Folder}");

            if (string.IsNullOrEmpty(Folder))
            {
                Result = "Empty";
            }
            else
            {
                var di = new DirectoryInfo(Path.GetDirectoryName(Folder));
                if (!di.Exists)
                {
                    Result = "Directory Not Found";
                }
                else
                {
                    var files = Directory
                        .GetFiles(Folder)
                        .Select(f => Path.GetFileName(f));

                    Result = string.Join("|", files);
                }
            }
        }
    }
}