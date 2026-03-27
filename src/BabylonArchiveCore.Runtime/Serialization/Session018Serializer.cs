using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация контракта S018.
/// </summary>
public static class Session018Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session018CommandContourContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session018CommandContourContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session018CommandContourContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session018CommandContourContract.");
        }

        return parsed;
    }
}
