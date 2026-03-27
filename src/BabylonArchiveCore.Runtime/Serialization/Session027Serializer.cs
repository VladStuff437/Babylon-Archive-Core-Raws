using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта боевого ввода S027.
/// </summary>
public sealed class Session027Serializer
{
    public string Serialize(Session027CombatInputContract state) => JsonSerializer.Serialize(state);

    public Session027CombatInputContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session027CombatInputContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
