namespace MeadowApp
{
    public class Config
    {
        public static string DefaultTestAssemblyFolder = "/meadow0/test";

        public Config()
        {
            TestAssemblyFolder = DefaultTestAssemblyFolder;
        }

        public string Display { get; set; }
        public string TestAssemblyFolder { get; set; }
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
                    Display = null,
                    TestAssemblyFolder = DefaultTestAssemblyFolder,
                    Network = new NetworkConfig
                    {
                        SSID = "MeadowNetwork",
                        Pass = "passphrase"
                    }
                };
            }
        }
    }
}