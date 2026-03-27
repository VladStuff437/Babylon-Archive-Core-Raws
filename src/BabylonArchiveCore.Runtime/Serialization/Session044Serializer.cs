using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Serializer for S044 fallback mission contract.
/// </summary>
public sealed class Session044Serializer
{
    public string Serialize(Session044FallbackMissionContract state) => JsonSerializer.Serialize(state);

    public Session044FallbackMissionContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session044FallbackMissionContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
