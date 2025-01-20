using RepoMonitor;

namespace MonitorTests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var updater = new OsUpdater("COM6", "1.15.0.0");
        updater.Connect();
        var versionIsRight = await updater.IsDesiredVersion();
        await updater.StartUpdate();
    }
}