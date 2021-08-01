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
    public class Config
    {
        public NetworkConfig Network { get; set; }

        public class NetworkConfig
        {
            public string SSID { get; set; }
            public string Pass { get; set; }
        }

        public static Config Default
        {
            get
            {
                return new Config
                {
                    Network = new NetworkConfig
                    {
                        SSID = "MeadowNetwork",
                        Pass = "passphrase"
                    }
                };
            }
        }
    }

    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        private Worker Worker { get; }

        public MeadowApp()
        {
            Console.WriteLine("=== Meadow TestSuite Worker ===");

            Worker = new Worker(Device);
            Worker.Start();
            // the above blocks, so we never actually get here

            Thread.Sleep(Timeout.Infinite);
        }

        private Config LoadConfig()
        {
            var fileName = "config.json";

            Console.WriteLine($" Loading configuration from {fileName}....");

            var fi = new FileInfo(fileName);
            if(!fi.Exists)
            {
                Console.WriteLine($" ! Configuration file not found at '{fi.FullName}'");
                return Config.Default;
            }

            
            var cfg = SimpleJson.SimpleJson.DeserializeObject<Config>(File.ReadAllText(fi.FullName));
            return cfg;
        }
    }
}