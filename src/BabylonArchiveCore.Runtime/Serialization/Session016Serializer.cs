using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация контракта S016.
/// </summary>
public static class Session016Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session016GameLoopContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session016GameLoopContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session016GameLoopContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session016GameLoopContract.");
        }

        return parsed;
    }
}
