using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта репутаций S035.
/// </summary>
public sealed class Session035Serializer
{
    public string Serialize(Session035FactionReputationContract state) => JsonSerializer.Serialize(state);

    public Session035FactionReputationContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session035FactionReputationContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
