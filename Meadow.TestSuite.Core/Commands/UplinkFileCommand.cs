using System;
using System.IO;

namespace Meadow.TestSuite
{
    public class UplinkFileCommand : TestCommand
    {
        public UplinkFileCommand()
        {
            CommandType = CommandType.UplinkFile;
        }

        public string Destination { get; set; }
        public string FileData { get; set; }

        public override void Execute(IWorker worker)
        {
            Console.WriteLine($" Data: {FileData.Length} Base64 chars");
            Console.WriteLine($" Destination: {Destination}");

            if (string.IsNullOrEmpty(Destination))
            {
                Console.WriteLine($" Invalid/missing file destination");
            }
            else
            {
                var di = new DirectoryInfo(Path.GetDirectoryName(Destination));
                if (!di.Exists)
                {
                    Console.WriteLine($" Creating directory {di.FullName}");
                    di.Create();
                }
                var data = Convert.FromBase64String(FileData);
                var fi = new FileInfo(Destination);
                if (fi.Exists)
                {
                    Console.WriteLine("Destination file exists.  Overwriting.");
                    fi.Delete();
                }
                
                File.WriteAllBytes(Destination, data);

                fi.Refresh();

                if (worker == null)
                {
                    Console.WriteLine($"Worker is null");
                }
                else if (worker.Registry == null)
                {
                    Console.WriteLine($"Worker Registry is null");
                }
                else
                {
                    worker.Registry.RegisterAssembly(Destination);
                }

                Console.WriteLine($"Destination file verified to be {fi.Length} bytes.");

                Result = "File written.";
            }
        }
    }
}