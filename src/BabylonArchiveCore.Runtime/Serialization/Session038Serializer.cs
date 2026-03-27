using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта MissionNode S038.
/// </summary>
public sealed class Session038Serializer
{
    public string Serialize(Session038MissionNodeContract state) => JsonSerializer.Serialize(state);

    public Session038MissionNodeContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session038MissionNodeContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
