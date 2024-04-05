using Meadow.CLI;
using Meadow.CLI.Commands.DeviceManagement;
using Meadow.Hcom;
using Meadow.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.TestSuite;

public class HcomTestDirector : ITestDirector
{
    private readonly string _route;

    private static MeadowConnectionManager ConnectionManager { get; }
    private IPackageManager PackageManager { get; }

    private readonly IMeadowConnection _connection;
    private readonly DirectoryInfo _testSourceRootFolder;

    private List<TestResult> _results = new();
    private TestResult? _activeTest = null;
    private CancellationTokenSource _testCompletionTokenSource = new();

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
        var routeproc = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                FileName = "meadow",
                Arguments = $"config route \"{_route}\""
            }
        };

        routeproc.Start();
        routeproc.WaitForExit();

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
                RedirectStandardInput = true,
            }
        };

        process.OutputDataReceived += (s, e) =>
        {
            if (e.Data != null)
            {
                var d = e.Data
                .Replace("stdout>", string.Empty)
                .Trim();

                // this avoids printing garbage like the spinner
                if (d.Length > 1)
                {
                    Console.WriteLine(e.Data);
                }

                // TODO: look for app termination and cancel if it happens

                if (d == "Initializing OS...")
                {
                    // the device os is up - start capturing?
                }
                else if (d.StartsWith(">>>"))
                {
                    // this is a test output!
                    var testInfo = d.Substring(3).TrimEnd('<').Trim();

                    if (testInfo == "START TEST SET")
                    {
                        Console.WriteLine("Starting new test set");
                        _results.Clear();
                    }
                    else if (testInfo == "END TEST SET")
                    {
                        _testCompletionTokenSource.Cancel();
                    }
                    else if (testInfo.StartsWith("BEGIN:"))
                    {
                        _activeTest = new TestResult
                        {
                            TestID = testInfo[6..].Trim(),
                            State = TestState.Running
                        };
                        Console.WriteLine($"Starting test {_activeTest.TestID}");
                    }
                    else if (testInfo == "SUCCESS")
                    {
                        if (_activeTest == null)
                        {
                        }
                        else
                        {
                            _activeTest.State = TestState.Success;
                            _results.Add(_activeTest);
                            Console.WriteLine($"test {_activeTest.TestID} succeeded");
                        }
                    }
                    else if (testInfo == "FAIL")
                    {
                        if (_activeTest == null)
                        {
                        }
                        else
                        {
                            _activeTest.State = TestState.Failed;
                            _results.Add(_activeTest);
                            Console.WriteLine($"test {_activeTest.TestID} failed");
                        }
                    }
                    else
                    {
                        if (_activeTest == null)
                        {
                        }
                        else
                        {
                            _activeTest.Output.Add(testInfo);
                        }
                    }
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

        try
        {
            await process.WaitForExitAsync(_testCompletionTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            // this is expected!
        }

        Console.WriteLine($"Received {_results.Count} test results:");
        foreach (var r in _results)
        {
            Console.WriteLine($"  {r.TestID}: {r.State}");
        }


        var result = new TestResult
        {
            State = TestState.Inconclusive
        };

        if (process.HasExited && process.ExitCode != 0)
        {
            if (process.ExitCode == 8)
            {
                Console.WriteLine("Process is already open (close the active console)");
            }
            else
            {
                Console.WriteLine($"app run returned {process.ExitCode}");
            }

            // typically happens when there was a problem running
            result.Output.Add($"app run returned {process.ExitCode}");
            return result;
        }
        else
        {
            // the test app ran and we're in the middle of a "listen"
            // this sends a <CTRL-C>
            process.StandardInput.Write("\x3");
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
