namespace MeadowApp
{
    public class Config
    {
        public string Display { get; set; }
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
}