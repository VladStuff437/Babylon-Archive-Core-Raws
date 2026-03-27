using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация S013-контракта версионирования схем.
/// </summary>
public static class Session013Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session013SchemaVersionContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session013SchemaVersionContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session013SchemaVersionContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session013SchemaVersionContract.");
        }

        return parsed;
    }
}
