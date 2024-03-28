using Meadow.CLI;
using Meadow.CLI.Commands.DeviceManagement;
using Meadow.Hcom;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Meadow.TestSuite;

public class HcomTestDirector : ITestDirector
{
    private readonly string _route;

    private static MeadowConnectionManager ConnectionManager { get; }

    private readonly IMeadowConnection _connection;

    static HcomTestDirector()
    {
        ConnectionManager = new MeadowConnectionManager(new SettingsManager());
    }

    public HcomTestDirector(string hcomRoute = "/dev/ttyACM0")
    {
        if (hcomRoute.StartsWith("hcom:"))
        {
            _route = hcomRoute[5..];
        }
        else
        {
            _route = hcomRoute;
        }

        var connection = ConnectionManager.GetConnectionForRoute(_route);
        if (connection == null)
        {
            throw new Exception();
        }
        _connection = connection;
    }

    public Task<TestResult> ExecuteTest(string testName)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> GetAssemblies()
    {
        throw new NotImplementedException();
    }

    public Task<WorkerInfo> GetInfo()
    {
        throw new NotImplementedException();
    }

    public Task<string[]> GetTestNames()
    {
        throw new NotImplementedException();
    }

    public Task<TestResult[]> GetTestResults()
    {
        throw new NotImplementedException();
    }

    public Task<TestResult[]> GetTestResults(string testID)
    {
        throw new NotImplementedException();
    }

    public Task<TestResult> GetTestResults(Guid resultID)
    {
        throw new NotImplementedException();
    }

    public Task<DateTime> GetTime()
    {
        throw new NotImplementedException();
    }

    public async Task SendDirectory(DirectoryInfo source, string? destinationDirectory)
    {
        if (_connection.Device == null)
        {
            await _connection.Attach();
        }

        var rtState = await _connection.Device.IsRuntimeEnabled();

        await _connection.Device.RuntimeDisable();

        foreach (var file in source.GetFiles())
        {
            try
            {
                var dest = destinationDirectory ?? "/meadow0";
                dest = Path.Combine(dest, file.Name).Replace('\\', '/');
                await _connection.Device.WriteFile(file.FullName, dest);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        if (rtState) _connection.Device.RuntimeEnable();
    }

    public Task SendFile(FileInfo source, string destinationName)
    {
        throw new NotImplementedException();
    }

    public Task SetTime(DateTime time)
    {
        throw new NotImplementedException();
    }
}
