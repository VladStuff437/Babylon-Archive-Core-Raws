using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта атрибутов S024.
/// </summary>
public sealed class Session024Serializer
{
    public string Serialize(Session024CharacterAttributesContract state) => JsonSerializer.Serialize(state);

    public Session024CharacterAttributesContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session024CharacterAttributesContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
