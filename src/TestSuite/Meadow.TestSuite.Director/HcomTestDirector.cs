using Meadow.CLI;
using Meadow.CLI.Commands.DeviceManagement;
using Meadow.Hcom;
using Meadow.Package;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Meadow.TestSuite;

public class HcomTestDirector : ITestDirector
{
    private readonly string _route;

    private static MeadowConnectionManager ConnectionManager { get; }
    private IPackageManager PackageManager { get; }

    private readonly IMeadowConnection _connection;
    private readonly DirectoryInfo _testSourceRootFolder;

    static HcomTestDirector()
    {
        ConnectionManager = new MeadowConnectionManager(new SettingsManager());
    }

    public HcomTestDirector(string testSourceRootFolder, string hcomRoute = "/dev/ttyACM0")
    {
        _testSourceRootFolder = new DirectoryInfo(testSourceRootFolder);

        if (!_testSourceRootFolder.Exists)
        {
            throw new Exception("Invalid source directory");
        }

        if (hcomRoute.StartsWith("hcom:"))
        {
            _route = hcomRoute[5..];
        }
        else
        {
            _route = hcomRoute;
        }
    }

    public Task<string[]> GetTestNames()
    {
        return Task.FromResult(
            new string[]
            {
                "TestA",
                "TestB"
            }
        );
    }

    public async Task<TestResult> ExecuteTest(string testName)
    {
        // TODO: build a a "test run" file

        // build and push the test
        var process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                FileName = "meadow",
                Arguments = $"app run \"{testName}\"",
                WorkingDirectory = _testSourceRootFolder.FullName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }
        };

        process.OutputDataReceived += (s, e) =>
        {
            if (e.Data != null)
            {
                var d = e.Data.TrimEnd();

                // this avoids printing garbage like the spinner
                if (d.Length > 1)
                {
                    Console.WriteLine(e.Data);
                }

                if (d == "Initializing OS...")
                {
                    // the device os is up - start capturing?
                }
                else if (d.StartsWith(">>>"))
                {
                    // this is a test output!
                }
            }
        };

        process.ErrorDataReceived += (s, e) =>
        {
            if (e.Data != null)
            {
                Console.WriteLine($"ERR: {e.Data}");
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        var result = new TestResult
        {
            State = TestState.Inconclusive
        };

        if (process.ExitCode != 0)
        {
            result.Output.Add($"app run returned {process.ExitCode}");
            return result;
        }

        // TODO: start a listener to wait for completion

        return result;
    }


    public Task<string[]> GetAssemblies()
    {
        throw new NotImplementedException();
    }

    public Task<WorkerInfo> GetInfo()
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

    public Task<bool> BuildTest(TestTarget target, string projectFilepath)
    {
        try
        {
            PackageManager.BuildApplication(projectFilepath);
            //            PackageManager.TrimApplication(projectFilepath);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            return Task.FromResult(false);
        }
    }
}
