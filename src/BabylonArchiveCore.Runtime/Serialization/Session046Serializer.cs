using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Serializer for S046 archive address contract.
/// </summary>
public sealed class Session046Serializer
{
    public string Serialize(Session046ArchiveAddressContract state) => JsonSerializer.Serialize(state);

    public Session046ArchiveAddressContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session046ArchiveAddressContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
