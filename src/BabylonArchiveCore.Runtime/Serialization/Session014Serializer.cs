using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация контракта S014.
/// </summary>
public static class Session014Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session014MigrationContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session014MigrationContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session014MigrationContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session014MigrationContract.");
        }

        return parsed;
    }
}
