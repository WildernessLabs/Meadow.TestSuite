using System.Text.Json;

namespace RepoMonitor;

internal class Program
{
    private static void Main(string[] args)
    {
        var json = File.ReadAllText("config.json");
        var settings = JsonSerializer.Deserialize<RepositorySettings>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

        var rs = new RepositoryService(settings);
        var ps = new PlatformService(rs, settings);

        while (true)
        {
            Thread.Sleep(1000);
        }

        Console.WriteLine("Hello, World!");
    }
}

