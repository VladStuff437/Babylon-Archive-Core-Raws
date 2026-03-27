using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Serializer for S041 reachability contract.
/// </summary>
public sealed class Session041Serializer
{
    public string Serialize(Session041ReachabilityContract state) => JsonSerializer.Serialize(state);

    public Session041ReachabilityContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session041ReachabilityContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
