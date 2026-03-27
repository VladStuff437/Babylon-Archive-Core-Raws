using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализация контракта S015.
/// </summary>
public static class Session015Serializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Serialize(Session015EventTaxonomyContract value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static Session015EventTaxonomyContract Deserialize(string json)
    {
        var parsed = JsonSerializer.Deserialize<Session015EventTaxonomyContract>(json, SerializerOptions);
        if (parsed is null)
        {
            throw new InvalidOperationException("Unable to deserialize Session015EventTaxonomyContract.");
        }

        return parsed;
    }
}
