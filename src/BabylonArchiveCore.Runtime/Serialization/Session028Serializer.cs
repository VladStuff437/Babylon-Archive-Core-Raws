using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта автоатаки S028.
/// </summary>
public sealed class Session028Serializer
{
    public string Serialize(Session028AutoAttackContract state) => JsonSerializer.Serialize(state);

    public Session028AutoAttackContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session028AutoAttackContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
