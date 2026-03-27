using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Serializer for S045 mission persistence contract.
/// </summary>
public sealed class Session045Serializer
{
    public string Serialize(Session045MissionPersistenceContract state) => JsonSerializer.Serialize(state);

    public Session045MissionPersistenceContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session045MissionPersistenceContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
