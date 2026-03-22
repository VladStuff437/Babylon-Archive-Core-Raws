using System.Text.Json;
using BabylonArchiveCore.Domain;

namespace BabylonArchiveCore.Infrastructure.Config;

public sealed class ConfigurationStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public GameConfiguration LoadOrCreateDefault(string filePath)
    {
        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            var existing = JsonSerializer.Deserialize<GameConfiguration>(content, JsonOptions);
            if (existing is not null)
            {
                return existing;
            }
        }

        var config = new GameConfiguration();
        Save(filePath, config);
        return config;
    }

    public void Save(string filePath, GameConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var json = JsonSerializer.Serialize(configuration, JsonOptions);
        File.WriteAllText(filePath, json);
    }
}
