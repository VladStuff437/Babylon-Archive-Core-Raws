using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта combat core S026.
/// </summary>
public sealed class Session026Serializer
{
    public string Serialize(Session026CombatCoreContract state) => JsonSerializer.Serialize(state);

    public Session026CombatCoreContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session026CombatCoreContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
