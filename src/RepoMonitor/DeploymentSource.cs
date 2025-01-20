using System.Text.Json;
using System.Text.Json.Serialization;

namespace RepoMonitor;

public class RepositorySettings
{
    [JsonPropertyName("packages")]
    public List<DeploymentPackage> Packages { get; set; }

    [JsonPropertyName("actions")]
    public List<ActionBase> Actions { get; set; }
}

[JsonConverter(typeof(ActionJsonConverter))]
public abstract class ActionBase
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("solutionPath")]
    public string SolutionPath { get; set; }
}

public class BuildAction : ActionBase
{
    public BuildAction()
    {
        Type = "build";
    }
}

public class DeployAction : ActionBase
{
    public DeployAction()
    {
        Type = "deploy";
    }

    [JsonPropertyName("route")]
    public string Route { get; set; }

    [JsonPropertyName("deployProject")]
    public string DeployProject { get; set; }
}

public class ActionJsonConverter : JsonConverter<ActionBase>
{
    public override ActionBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument document = JsonDocument.ParseValue(ref reader))
        {
            var root = document.RootElement;
            var type = root.GetProperty("type").GetString();

            return type?.ToLower() switch
            {
                "build" => JsonSerializer.Deserialize<BuildAction>(root.GetRawText(), options),
                "deploy" => JsonSerializer.Deserialize<DeployAction>(root.GetRawText(), options),
                _ => throw new JsonException($"Unknown action type: {type}")
            };
        }
    }

    public override void Write(Utf8JsonWriter writer, ActionBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

public class Repo : IEquatable<Repo>
{
    public string Owner { get; set; }
    public string Repository { get; set; }
    public string Branch { get; set; }
    public bool Watch { get; set; }
    public bool Pull { get; set; }
    public string? PAT { get; set; }

    public bool Equals(Repo? other)
    {
        if (other == null) return false;

        if (Owner != other.Owner) return false;
        if (Repository != other.Repository) return false;
        if (Branch != other.Branch) return false;

        return true;
    }
    public override bool Equals(object? obj)
    {
        return Equals(obj as Repo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Owner, Repository, Branch);
    }
}

public class DeploymentPackage
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("repositories")]
    public List<Repo> Repositories { get; set; }
}