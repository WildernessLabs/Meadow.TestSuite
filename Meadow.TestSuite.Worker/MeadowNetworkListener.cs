using Meadow.Foundation.Web.Maple.Server;
using Meadow.TestSuite;
using System;
using System.Net;

namespace MeadowApp
{
    public class MeadowNetworkListener : ITestListener
    {
        private MapleServer _server;
        private ILogger _logger;

        public MeadowNetworkListener(IPAddress address, Config config, ICommandSerializer serializer, ILogger logger)
        {            
            _server = new MapleServer(address, 8080, false, RequestProcessMode.Serial, logger);
            _logger = logger;
        }

        public event CommandReceivedHandler CommandReceived;

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