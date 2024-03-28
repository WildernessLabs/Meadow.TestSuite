using Smee.IO.Client;
using System.Text.Json;

namespace TestSuite.Coordinator;

public class WebhookSmeeProxy
{
    private SmeeClient _client;

    public WebhookSmeeProxy()
    {
        _client = new SmeeClient(new Uri("https://smee.io/aO5SkwCevWoV7AXU"));
        _client.OnConnect += (sender, a) => Console.WriteLine($"Webhook Proxy Connected");
        _client.OnDisconnect += (sender, a) => Console.WriteLine($"Webhook Proxy Disconnected");
        _client.OnMessage += (sender, smeeEvent) =>
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            var json = smeeEvent.Data.Body.ToString();
            //json = json.Substring(1, json.Length - 2); // the whole thing is wrapped in "{...}".
            var evt = JsonSerializer.Deserialize<PullRequestEvent>(json);
            Console.WriteLine($"Pull Request #{evt!.Number} {evt!.Action}");
            if (evt.Action == "opened" || evt.Action == "reopened")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  Repo:{evt!.Repository.Name}");
                Console.WriteLine($"  branch:{evt!.PullRequest.Head.Ref}");
                Console.WriteLine($"  Clone URL:{evt!.Repository.CloneUrl}");
            }

            Console.ResetColor();
            Console.WriteLine();
        };
        //_client.OnPing += (sender, a) => Console.WriteLine($"Ping from Smee{Environment.NewLine}");
        _client.OnError += (sender, e) => Console.WriteLine($"Error was raised (Disconnect/Anything else: {e.Message}{Environment.NewLine}");
        _ = _client.StartAsync();
    }
}
