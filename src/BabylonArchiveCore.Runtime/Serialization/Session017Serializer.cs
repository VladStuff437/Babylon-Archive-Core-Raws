using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация контракта S017.
/// </summary>
public static class Session017Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session017RuntimeStateContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session017RuntimeStateContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session017RuntimeStateContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session017RuntimeStateContract.");
        }

        return parsed;
    }
}
