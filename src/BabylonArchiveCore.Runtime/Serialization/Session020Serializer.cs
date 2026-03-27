using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация контракта S020.
/// </summary>
public static class Session020Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session020ControlProfilesContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session020ControlProfilesContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session020ControlProfilesContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session020ControlProfilesContract.");
        }

        return parsed;
    }
}
