using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Serializer for S042 dead-end contract.
/// </summary>
public sealed class Session042Serializer
{
    public string Serialize(Session042DeadEndContract state) => JsonSerializer.Serialize(state);

    public Session042DeadEndContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session042DeadEndContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
