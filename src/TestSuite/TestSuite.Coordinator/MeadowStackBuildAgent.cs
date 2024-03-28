using System.Diagnostics;

namespace TestSuite.Coordinator;

public class MeadowStackBuildAgent
{
    private readonly DirectoryInfo _root;
    private readonly IEnumerable<string> _repositoryStack;

    public MeadowStackBuildAgent(IEnumerable<string> repositoryStack, DirectoryInfo root, string branch = "develop")
    {
        _repositoryStack = repositoryStack;
        _root = root;

        if (!_root.Exists)
        {
            _root.Create();
        }
    }

    public bool Build(string solutionFile, string configuration = "Release")
    {
        var solutionFilePath = Path.Combine(_root.FullName, solutionFile);

        if (!File.Exists(solutionFilePath))
        {
            return false;
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = $"build -c {configuration} {solutionFilePath}",
                WorkingDirectory = _root.FullName
            }
        };
        process.Start();
        process.WaitForExit();

        return process.ExitCode == 0;
    }

    public bool CloneTree(string branch = "main")
    {
        var success = true;
        foreach (var repo in _repositoryStack)
        {
            var repoFolder = Path.Combine(_root.FullName, repo);
            if (Directory.Exists(repoFolder))
            {
                Directory.Delete(repoFolder, true);
            }

            success &= CloneRepo(repo);
            success &= ChangeBranch(repo, branch);
        }
        return success;
    }

    public bool PullTree(string branch = "develop")
    {
        var success = true;

        foreach (var repo in _repositoryStack)
        {
            var repoFolder = Path.Combine(_root.FullName, repo);

            if (!Directory.Exists(repoFolder))
            {
                success &= CloneRepo(repo);
            }

            success &= ChangeBranch(repo, branch);
        }

        return success;
    }

    private bool CleanRepo(string repo)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = $"clean -dfx",
                WorkingDirectory = Path.Combine(_root.FullName, repo),
            }
        };
        process.Start();
        process.WaitForExit();

        return process.ExitCode == 0;
    }

    private bool ChangeBranch(string repo, string branch)
    {
        if (!CleanRepo(repo))
        {
            return false;
        }

        // git fetch origin
        // git switch -c test origin/test
        var process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = $"fetch origin",
                WorkingDirectory = Path.Combine(_root.FullName, repo),
            }
        };
        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            return false;
        }

        process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = $"switch -c {branch} origin/{branch}",
                WorkingDirectory = Path.Combine(_root.FullName, repo),
            }
        };
        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            return false;
        }

        process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = $"pull",
                WorkingDirectory = Path.Combine(_root.FullName, repo),
            }
        };
        process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }

    private bool CloneRepo(string repoName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = $"clone https://github.com/WildernessLabs/{repoName}.git",
                WorkingDirectory = _root.FullName,
            }
        };
        process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }
}
