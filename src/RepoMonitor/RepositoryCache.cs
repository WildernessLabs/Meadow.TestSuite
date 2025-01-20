using System.Text.Json;

namespace RepoMonitor;

internal class RepositoryCache
{
    private Dictionary<string, string> _commitCache = new();
    private Timer _persistTimer;

    private string StoreDirectory { get; }
    private string StoreFile { get; }

    public RepositoryCache()
    {
        StoreDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RepoMonitor");
        if (!Directory.Exists(StoreDirectory))
        {
            Directory.CreateDirectory(StoreDirectory);
        }
        StoreFile = Path.Combine(StoreDirectory, "commit-cache.json");

        LoadPersistentCache();

        _persistTimer = new Timer(new TimerCallback(PersistCache));
    }

    private void LoadPersistentCache()
    {
        if (File.Exists(StoreFile))
        {
            var json = File.ReadAllText(StoreFile);
            _commitCache = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
    }

    public bool AddOrUpdate(string repositoryKey, string latestSha)
    {
        var changed = false;

        if (_commitCache.ContainsKey(repositoryKey))
        {
            if (_commitCache[repositoryKey] != latestSha)
            {
                _commitCache[repositoryKey] = latestSha;
                changed = true;
            }
        }
        else
        {
            _commitCache.Add(repositoryKey, latestSha);
            changed = true;
        }

        if (changed)
        {
            _persistTimer.Change(5000, -1);
        }

        return changed;
    }

    private void PersistCache(object? _)
    {
        var json = JsonSerializer.Serialize(_commitCache, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        });

        File.WriteAllText(StoreFile, json);
    }
}
