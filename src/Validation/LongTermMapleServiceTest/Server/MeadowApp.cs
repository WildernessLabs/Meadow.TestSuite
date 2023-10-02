using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using Meadow.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace LongTermMapleServiceTest;

public partial class MeadowApp : App<F7FeatherV2>
{
    private MapleServer _mapleServer = default!;

    private const string SSID = "BOBS_YOUR_UNCLE";
    private const string PASSCODE = "1234567890";
    private const bool ManualGCCollect = false;

    public override Task Initialize()
    {
        Resolver.Log.Info("LTMST Server Initializing");

        // add an LED for the handler to blink
        var led = Device.Pins.OnboardLedGreen.CreateDigitalOutputPort();
        Resolver.Services.Add(led);

        // add a memory monitor
        Resolver.Services.Add(new MemoryMonitor(TimeSpan.FromSeconds(30), ManualGCCollect));

        var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

        if (wifi == null)
        {
            Resolver.Log.Error("No WiFi Adapter!");
        }
        else
        {
            wifi.NetworkConnected += (s, e) =>
            {
                Resolver.Log.Error("Network connected.");

                // add a UDP log provider
                Resolver.Log.AddProvider(new UdpLogger());

                // create the maple server
                _mapleServer = new MapleServer(IPAddress.Any, 8080)
                {
                    Advertise = false
                };
                _mapleServer.Start();
            };

            wifi.Connect(SSID, PASSCODE);
        }

        return base.Initialize();
    }


}
