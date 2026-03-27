using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация S012-схемы сейва.
/// </summary>
public static class Session012Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session012SaveContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session012SaveContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session012SaveContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session012SaveContract.");
        }

        return parsed;
    }
}
