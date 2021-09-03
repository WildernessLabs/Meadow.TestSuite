using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.TestSuite;
using System;
using System.Net;

namespace MeadowApp
{
    public class TestRequestHandler : RequestHandlerBase
    {
        public override bool IsReusable => true;

        [HttpGet]
        public void Tests()
        {
            Console.WriteLine("GET Tests");
        }
    }

    public class MeadowNetworkListener : ITestListener
    {
        MapleServer _server;

        public MeadowNetworkListener(IPAddress address, Config config, ICommandSerializer serializer)
        {            
            _server = new MapleServer(address, 8080);
        }

        public event CommandReceivedHandler CommandReceived;

        public void SendResult(object result)
        {
            Console.WriteLine("+MeadowNetworkListener.SendResult");
        }

        public void StartListening()
        {
            Console.WriteLine("+MeadowNetworkListener.StartListening");
            Console.WriteLine("Starting Maple Server");
            _server.Start();
        }
    }
}