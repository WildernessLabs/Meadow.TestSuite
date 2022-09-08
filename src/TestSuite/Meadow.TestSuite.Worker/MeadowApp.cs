using Meadow;
using Meadow.Devices;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowApp
{
    public class MeadowApp : App<F7FeatherV2>
    {
        public static Worker Worker { get; private set; }

        public override async Task Initialize()
        {
            Console.WriteLine("=== Meadow TestSuite Worker ===");

            Worker = new Worker(Device);
            var cfg = LoadConfig();
            await Worker.Configure(cfg);

            Worker.Start();

            Thread.Sleep(Timeout.Infinite);
        }

        private Config LoadConfig()
        {
            var fileName = "config.json";

            Console.WriteLine($" Loading configuration from {fileName}....");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            var fi = new FileInfo(path);
            if (!fi.Exists)
            {
                Console.WriteLine($" ! Configuration file not found at '{fi.FullName}'");
                return Config.Default;
            }


            Console.WriteLine($" Deserializing config...");
            var cfg = SimpleJson.SimpleJson.DeserializeObject<Config>(File.ReadAllText(fi.FullName));
            return cfg;
        }
    }
}