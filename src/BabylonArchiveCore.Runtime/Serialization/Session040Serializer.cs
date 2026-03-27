using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор runtime-контракта миссии S040.
/// </summary>
public sealed class Session040Serializer
{
    public string Serialize(Session040MissionRuntimeContract state) => JsonSerializer.Serialize(state);

    public Session040MissionRuntimeContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session040MissionRuntimeContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
