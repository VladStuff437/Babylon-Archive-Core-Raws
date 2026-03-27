using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация S011-контракта данных.
/// </summary>
public static class Session011Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session011DataContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session011DataContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session011DataContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session011DataContract.");
        }

        return parsed;
    }
}
