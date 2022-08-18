using System;
using System.IO;

namespace Meadow.TestSuite
{
    public class DeleteFileCommand : TestCommand
    {
        public string Path { get; set; }
        public string Pattern { get; set; }

        public DeleteFileCommand()
        {
            CommandType = CommandType.DeleteAssemblies;
        }

        public override void Execute(IWorker worker)
        {
            string[] files;
            var count = 0;

            if(!string.IsNullOrEmpty(Pattern))
            {
                files = Directory.GetFiles(Path, Pattern);
            }
            else
            {
                files = Directory.GetFiles(Path);
            }

            foreach (var f in files)
            {
                try
                {
                    File.Delete(f);
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            worker.Registry.Clear();

            Result = $"{count} files deleted";
        }
    }
}