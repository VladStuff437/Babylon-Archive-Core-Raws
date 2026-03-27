using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация контракта S019.
/// </summary>
public static class Session019Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session019ActionMapContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session019ActionMapContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session019ActionMapContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session019ActionMapContract.");
        }

        return parsed;
    }
}
