using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта enemy state machine S030.
/// </summary>
public sealed class Session030Serializer
{
    public string Serialize(Session030EnemyStateMachineContract state) => JsonSerializer.Serialize(state);

    public Session030EnemyStateMachineContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session030EnemyStateMachineContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
