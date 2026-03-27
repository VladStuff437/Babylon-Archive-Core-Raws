using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта перцепции S029.
/// </summary>
public sealed class Session029Serializer
{
    public string Serialize(Session029PerceptionContract state) => JsonSerializer.Serialize(state);

    public Session029PerceptionContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session029PerceptionContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
