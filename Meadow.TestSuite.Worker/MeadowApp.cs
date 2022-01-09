using Meadow;
using Meadow.Devices;
using Meadow.TestSuite;
using SimpleJson;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        public static Worker Worker { get; private set; }

        public MeadowApp()
        {
            Console.WriteLine("=== Meadow TestSuite Worker ===");

            Worker = new Worker(Device);
            var cfg = LoadConfig();
            Worker.Configure(cfg);

            Worker.Start();

            Thread.Sleep(Timeout.Infinite);
        }

        private Config LoadConfig()
        {
            var fileName = "config.json";

            Console.WriteLine($" Loading configuration from {fileName}....");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            var fi = new FileInfo(path);
            if(!fi.Exists)
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