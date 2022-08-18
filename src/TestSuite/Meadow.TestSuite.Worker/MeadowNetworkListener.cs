using Meadow.Foundation.Web.Maple.Server;
using Meadow.Logging;
using Meadow.TestSuite;
using System.Net;

namespace MeadowApp
{
    public class MeadowNetworkListener : ITestListener
    {
        public event CommandReceivedHandler CommandReceived;

        private MapleServer _server;
        private Logger _logger;

        public MeadowNetworkListener(IPAddress address, Config config, ICommandSerializer serializer, Logger logger)
        {            
            _server = new MapleServer(address, 8080, false, RequestProcessMode.Serial, logger);
            _logger = logger;
        }

        public void SendResult(object result)
        {
            _logger?.Info("+MeadowNetworkListener.SendResult");
        }

        public void StartListening()
        {
            _logger?.Info("+MeadowNetworkListener.StartListening");
            _logger?.Info("Starting Maple Server");
            _server.Start();
            _logger?.Info("-MeadowNetworkListener.StartListening");
        }
    }
}