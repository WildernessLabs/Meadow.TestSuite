using Meadow.Devices;
using Meadow.Hardware;
using System.Threading.Tasks;
using Validation;

namespace Meadow.Validation;

public class MeadowApp : ProjectLabCoreComputeApp
{
    private TestService _testService;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize hardware...");

        Resolver.Log.Info($"Running on ProjectLab Hardware {Hardware.RevisionString}");

        _testService = new TestService(Hardware);

        var ssid = Settings["TestRecorder.WiFi.Ssid"];
        var passphrase = Settings["TestRecorder.WiFi.Passphrase"];

        // TODO: show "connecting" on UI

        Resolver.Log.Info($"Connecting to SSID '{ssid}'...");
        var wifi = Hardware.ComputeModule.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
        wifi.NetworkConnected += OnNetworkConnected;

        _ = wifi.Connect(ssid, passphrase);

        return base.Initialize();
    }

    private void OnNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
    {
        Resolver.Log.Info($"Network connected!");

        // the fact we connected means WiFi works, so we might as well report that as a test success
        _testService.AddTestInfo(new TestInfo("Manual WiFi Connect", TestResult.Pass));

        _ = _testService.StartTests();
    }
}
