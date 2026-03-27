using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта MissionDefinition S037.
/// </summary>
public sealed class Session037Serializer
{
    public string Serialize(Session037MissionDefinitionContract state) => JsonSerializer.Serialize(state);

    public Session037MissionDefinitionContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session037MissionDefinitionContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
