using LibGit2Sharp;
using Octokit;
using System.Diagnostics;

namespace RepoMonitor;


// monitors the repo, raises events when they change, pulls code, etc.
public class RepositoryService
{
    public event EventHandler<DeploymentPackage[]>? WatchedRepositoryChanged;

    private Timer _timer;
    private TimeSpan _pollInterval = TimeSpan.FromSeconds(60);
    private GitHubClient _client;
    private RepositoryCache _packageCommitCache = new();
    private RepositorySettings _settings;

    public DirectoryInfo RepoRoot { get; private set; }

    public RepositoryService(RepositorySettings settings)
    {
        _ = Initialize(settings);
    }

    private async Task Initialize(RepositorySettings settings)
    {
        _settings = settings;

        _client = new GitHubClient(new ProductHeaderValue("dotnetMakers"));

        RepoRoot = new DirectoryInfo($"f:\\temp\\repo-monitor");

        if (!RepoRoot.Exists)
        {
            RepoRoot.Create();
        }

        // TODO: handle auth for private repos

        // fire up a monitor timer immediately;
        _timer = new Timer(CheckForChanges, null, 0, -1);
    }

    private async void CheckForChanges(object? state)
    {
        try
        {
            var changeList = new List<string>();
            var watchList = new List<DeploymentPackage>();

            foreach (var repo in _settings.Packages.SelectMany(p => p.Repositories))
            {
                var branch = await _client.Repository.Branch.Get(repo.Owner, repo.Repository, repo.Branch);

                var key = $"{repo.Owner}\\{repo.Repository}";
                var sha = branch.Commit.Sha;

                // TODO: we could get commit info with 
                var commit = await _client.Repository.Commit.Get(repo.Owner, repo.Repository, sha);
                var message = commit.Commit.Message;
                var author = commit.Commit.Author.Name;
                var date = commit.Commit.Author.Date;

                if (_packageCommitCache.AddOrUpdate(key, sha))
                {
                    if (repo.Watch)
                    {
                        var p = _settings.Packages.Where(p => p.Repositories.Contains(repo));
                        // this is a trigger
                        watchList.AddRange(p.Except(watchList));
                    }

                    changeList.Add(key);
                }
            }

            if (changeList.Count > 0)
            {
                Debug.WriteLine($"{changeList.Count} repos have changed");
            }

            if (watchList.Count > 0)
            {
                WatchedRepositoryChanged?.Invoke(this, watchList.ToArray());
            }
        }
        catch (Exception ex)
        {
        }

        _timer.Change(_pollInterval, TimeSpan.FromMilliseconds(-1));
    }

    public void SyncPackage(DeploymentPackage package)
    {
        foreach (var repo in package.Repositories)
        {
            SyncRepository(repo);
        }
    }

    public void SyncRepository(Repo repo)
    {
        var repoUrl = $"https://github.com/{repo.Owner}/{repo.Repository}.git";
        var localPath = Path.Combine(RepoRoot.FullName, repo.Repository);

        FetchOptions? fetchOptions = null;

        if (repo.PAT != null)
        {
            fetchOptions = new FetchOptions
            {
                CredentialsProvider = (_, _, _) =>
                {
                    // Set up credentials
                    return new UsernamePasswordCredentials
                    {
                        Username = repo.PAT,  // For GitHub PAT, username is the token
                        Password = string.Empty  // Password is empty when using PAT
                    };
                }
            };
        }

        if (!Directory.Exists(localPath))
        {
            // Repository doesn't exist locally, clone it
            CloneOptions cloneOptions = new CloneOptions(fetchOptions)
            {
                BranchName = repo.Branch,
            };
            Console.WriteLine($"Cloning {repo.Repository}:{repo.Branch}...");
            LibGit2Sharp.Repository.Clone(repoUrl, localPath, cloneOptions);
            Console.WriteLine($"Repository cloned to {localPath}");
        }
        else
        {
            // Repository exists, reset it to match remote
            using (var repository = new LibGit2Sharp.Repository(localPath))
            {
                Console.WriteLine($"Pulling {repo.Repository}:{repo.Branch}...");

                // Fetch latest from remote
                var remote = repository.Network.Remotes["origin"];
                Commands.Fetch(repository, remote.Name, new string[] { }, fetchOptions, null);

                // Get the remote branch reference
                var remoteBranchRef = repository.Branches[$"origin/{repo.Branch}"];
                if (remoteBranchRef == null)
                {
                    throw new Exception($"Remote branch {repo.Branch} not found");
                }

                // Reset the current branch to match remote
                var localBranch = repository.Branches[repo.Branch] ?? repository.CreateBranch(repo.Branch, remoteBranchRef.Tip);
                if (!localBranch.IsCurrentRepositoryHead)
                {
                    Commands.Checkout(repository, localBranch);
                }

                // Hard reset to remote branch
                repository.Reset(ResetMode.Hard, remoteBranchRef.Tip);

                // Clean untracked files
                CleanWorkingDirectory(repository);

                Console.WriteLine($"Repository reset to origin/{repo.Branch}");
            }
        }
    }

    private static void CleanWorkingDirectory(LibGit2Sharp.Repository repository)
    {
        // Remove untracked files and directories
        var status = repository.RetrieveStatus();
        foreach (var file in status.Untracked)
        {
            var fullPath = Path.Combine(repository.Info.WorkingDirectory, file.FilePath);
            File.Delete(fullPath);
        }

        // Remove empty directories
        CleanEmptyDirectories(repository.Info.WorkingDirectory);
    }

    private static void CleanEmptyDirectories(string directory)
    {
        foreach (var dir in Directory.GetDirectories(directory))
        {
            CleanEmptyDirectories(dir);
            if (!Directory.EnumerateFileSystemEntries(dir).Any())
            {
                Directory.Delete(dir);
            }
        }
    }
}