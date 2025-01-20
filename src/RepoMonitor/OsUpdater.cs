using Meadow.CLI;
using Meadow.CLI.Commands.DeviceManagement;
using Meadow.Cloud.Client;
using Meadow.Cloud.Client.Identity;
using Meadow.Hcom;
using Meadow.Software;

namespace RepoMonitor;

// deploys new binaries (platform and app) to configured hardware

public class OsUpdater
{
    private ISettingsManager _settings;
    private IMeadowConnection _connection;
    private string _route;
    private Version _targetVersion;

    public OsUpdater(string deviceRoute, string targetVersion)
    {
        _route = deviceRoute;
        _targetVersion = Version.Parse(targetVersion);
    }

    public void Connect()
    {
        _settings = new SettingsManager();
        var cm = new MeadowConnectionManager(_settings);
        var connection = cm.GetConnectionForRoute(_route);

        _connection = connection;
    }

    public async Task<bool> IsDesiredVersion()
    {
        await _connection.Attach();
        var info = await _connection.GetDeviceInfo();

        var os = Version.Parse(info.OsVersion);
        var runtime = Version.Parse(info.RuntimeVersion);
        var esp = Version.Parse(info.CoprocessorOsVersion);

        return true;
    }

    public async Task StartUpdate()
    {
        var identityManager = new IdentityManager();
        var httpClient = new HttpClient();
        var userAgent = new MeadowCloudUserAgent("OS Updater");

        var mcc = new MeadowCloudClient(
            httpClient, identityManager, userAgent);

        var fm = new FileManager(mcc);
        await fm.Refresh();
        var collection = fm.Firmware["Meadow F7"];

        var target = collection.FirstOrDefault(p => p.Version == _targetVersion.ToString(4));

        //  collection.PackageFileRoot
        //         var dst = $"/meadow0/update/

        var files = new List<(string src, string dest)>();

        files.Add((Path.Combine(collection.PackageFileRoot, target.Version, target.OSWithBootloader),
            $"/meadow0/update/os/{target.OSWithBootloader}"));

        files.Add((Path.Combine(collection.PackageFileRoot, target.Version, target.Runtime),
            $"/meadow0/update/os/{target.Runtime}"));

        files.Add((Path.Combine(collection.PackageFileRoot, target.Version, target.CoprocApplication),
            $"/meadow0/update/firmware/{target.CoprocApplication}"));

        files.Add((Path.Combine(collection.PackageFileRoot, target.Version, target.CoprocBootloader),
            $"/meadow0/update/firmware/{target.CoprocBootloader}"));

        files.Add((Path.Combine(collection.PackageFileRoot, target.Version, target.CoprocPartitionTable),
            $"/meadow0/update/firmware/{target.CoprocPartitionTable}"));

        await _connection.RuntimeDisable();

        foreach (var file in files)
        {
            await _connection.WriteFile(file.src, file.dest);
        }

        // we're going to write the files to the OtA location and reset it

        //await _connection.WriteFile
    }
}
