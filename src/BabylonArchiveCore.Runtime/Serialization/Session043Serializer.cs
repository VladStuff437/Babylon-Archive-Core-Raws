using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Serializer for S043 cycle safety contract.
/// </summary>
public sealed class Session043Serializer
{
    public string Serialize(Session043CycleSafetyContract state) => JsonSerializer.Serialize(state);

    public Session043CycleSafetyContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session043CycleSafetyContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
