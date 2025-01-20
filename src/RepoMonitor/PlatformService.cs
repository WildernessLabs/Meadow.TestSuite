using Meadow.CLI;
using Meadow.CLI.Commands.DeviceManagement;
using Meadow.Cloud.Client;
using Meadow.Cloud.Client.Identity;
using Meadow.Hcom;
using Meadow.Package;
using Meadow.Software;
using System.Diagnostics;

namespace RepoMonitor;

public class AppDeployer
{
    private readonly string _route;
    private readonly string _projectPath;
    private ISettingsManager _settings;
    private IMeadowConnection _connection;

    public AppDeployer(string route, string projectPath)
    {
        _route = route;
        _projectPath = projectPath;
    }

    public async Task Deploy()
    {
        // TODO: call something to allow setting local state (lamp, UI, etc)

        _settings = new SettingsManager();
        var cm = new MeadowConnectionManager(_settings);
        var connection = cm.GetConnectionForRoute(_route);

        var identityManager = new IdentityManager();
        var httpClient = new HttpClient();
        var userAgent = new MeadowCloudUserAgent("App Deployer");

        var mcc = new MeadowCloudClient(
            httpClient, identityManager, userAgent);
        var fm = new FileManager(mcc);
        var pm = new PackageManager(fm);

        _connection = connection;

        // connect to the device
        await _connection.Attach();

        var deviceInfo = await _connection.GetDeviceInfo();

        // in order to deploy, the runtime must be disabled
        await _connection.Device.RuntimeDisable();

        try
        {
            pm.BuildApplication(_projectPath);
            Console.WriteLine("Building...");
            var appFile = PackageManager.GetAvailableBuiltConfigurations(
                Path.GetDirectoryName(_projectPath), "App.dll")
                .OrderByDescending(c => c.LastWriteTime)
                .First();

            Console.WriteLine("Trimming...");
            await pm.TrimApplication(
                appFile,
                deviceInfo.OsVersion);

            Console.WriteLine("Deploying...");
            await DeployApplication(
                pm,
                _connection,
                deviceInfo,
                _projectPath);
        }
        catch (Exception ex)
        {
        }

        // for now, assume the app will do logging to the cloud, etc.

        // TODO: call something to allow setting local state (lamp, UI, etc)

        // TODO: should we log the output, check status, etc?

        _connection.Device.RuntimeEnable();
    }

    private async Task<bool> DeployApplication(
        IPackageManager packageManager,
        IMeadowConnection connection,
        DeviceInfo deviceInfo,
        string path)
    {
        var configuration = "Release";

        connection.FileWriteProgress += OnFileWriteProgress;

        var candidates = PackageManager.GetAvailableBuiltConfigurations(path, "App.dll");

        if (candidates.Length == 0)
        {
            Console.WriteLine($"Cannot find a compiled application at '{path}'");
            return false;
        }

        //get the file that matches the configuration
        var file = candidates.FirstOrDefault(c => c.DirectoryName.Contains(configuration, StringComparison.OrdinalIgnoreCase));

        if (file == null)
        {
            Console.WriteLine($"Cannot find a compiled application for configuration '{configuration}'");
            return false;
        }

        Console.WriteLine($"Deploying app from {file.DirectoryName}...");

        await AppManager.DeployApplication(
            packageManager,
            connection,
            deviceInfo.OsVersion,
            file.DirectoryName!,
            true,
            false,
            null,
            CancellationToken.None);

        connection.FileWriteProgress -= OnFileWriteProgress;

        return true;
    }

    private void OnFileWriteProgress(object? sender, (string fileName, long completed, long total) e)
    {
        var p = e.completed / (double)e.total * 100d;

        if (!double.IsNaN(p))
        {
            Console.Write($"Writing  '{e.fileName}': {p:0}%         \r");
        }
    }
}

public class PlatformService
{
    private RepositoryService _repositoryService;
    private RepositorySettings _settings;

    public PlatformService(RepositoryService repositoryService, RepositorySettings settings)
    {
        _settings = settings;
        _repositoryService = repositoryService;
        _repositoryService.WatchedRepositoryChanged += OnWatchedRepositoryChanged;
    }

    private void OnWatchedRepositoryChanged(object? sender, DeploymentPackage[] e)
    {
        // we need to pull everything in the package
        foreach (var package in e)
        {
            Console.WriteLine($"Synchonizing '{package.Name}'");
            _repositoryService.SyncPackage(package);
        }

        Console.WriteLine($"All repositories synchronized.");

        // trigger builds
        new Thread(
            RunBuildActions
        ).Start();
    }

    private void RunBuildActions()
    {
        foreach (var action in _settings.Actions)
        {
            var buildTarget = Path.Combine(_repositoryService.RepoRoot.FullName, action.SolutionPath);

            Console.WriteLine($"Building '{buildTarget}'");

            BuildSolution(buildTarget);

            if (action is DeployAction da)
            {
                StartDeployment(da);
            }
        }
    }

    private Task StartDeployment(DeployAction deployAction)
    {
        // TODO: should we monitor this?  Maybe use TestSuite stuff?
        var path = Path.Combine(_repositoryService.RepoRoot.FullName, deployAction.DeployProject);
        var deployer = new AppDeployer(deployAction.Route, path);
        return deployer.Deploy();
    }

    public bool BuildSolution(string solutionPath)
    {
        int errorCount = 0;
        int warningCount = 0;

        try
        {
            if (!File.Exists(solutionPath))
            {
                throw new FileNotFoundException("Solution file not found", solutionPath);
            }

            Console.WriteLine($"\nCleaning solution: {Path.GetFileName(solutionPath)}");
            if (!RunDotNetCommand(solutionPath, "clean -c Release", ref errorCount, ref warningCount))
            {
                WriteColor("Clean failed!", ConsoleColor.Red);
                return false;
            }

            Console.WriteLine($"\nBuilding solution: {Path.GetFileName(solutionPath)}");
            if (!RunDotNetCommand(solutionPath, "build -c Release -v n", ref errorCount, ref warningCount))
            {
                WriteColor("Build failed!", ConsoleColor.Red);
                return false;
            }

            if (errorCount > 0 || warningCount > 0)
            {
                Console.WriteLine();

                WriteColor($"Total Errors: {errorCount}", ConsoleColor.Red);
                WriteColor($"Total Warnings: {warningCount}", ConsoleColor.Yellow);
            }

            if (errorCount == 0)
            {
                Console.WriteLine();
                WriteColor("Build completed successfully!", ConsoleColor.Green);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            WriteColor($"Build error: {ex.Message}", ConsoleColor.Red);
            return false;
        }
    }

    private bool RunDotNetCommand(string solutionPath, string arguments, ref int errorCount, ref int warningCount)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"{arguments} \"{solutionPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(solutionPath)
        };

        var ec = 0;
        var wc = 0;

        using (var process = new Process { StartInfo = startInfo })
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null) return;
                ProcessOutputLine(e.Data, ref ec, ref wc);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null) return;
                WriteColor(e.Data, ConsoleColor.Red);
                ec++;
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            errorCount = ec;
            warningCount = wc;

            return process.ExitCode == 0;
        }
    }

    private void ProcessOutputLine(string line, ref int errorCount, ref int warningCount)
    {
        // Common patterns in build output
        if (line.Contains(": error"))
        {
            WriteColor(line, ConsoleColor.Red);
            errorCount++;
        }
        else if (line.Contains(": warning"))
        {
            WriteColor(line, ConsoleColor.Yellow);
            warningCount++;
        }
        else if (line.StartsWith("Build succeeded") || line.Contains("0 Error(s)"))
        {
            WriteColor(line, ConsoleColor.Green);
        }
        else if (line.StartsWith("Build FAILED"))
        {
            WriteColor(line, ConsoleColor.Red);
        }
        // Project build started/completed
        else if (line.Contains("Building project") || line.Contains("Project") && line.Contains("-> "))
        {
            WriteColor(line, ConsoleColor.Cyan);
        }
        // Regular output
        else
        {
            Console.WriteLine(line);
        }
    }

    private static void WriteColor(string message, ConsoleColor color)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }
}