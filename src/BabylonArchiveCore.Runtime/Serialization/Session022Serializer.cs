using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта взаимодействий S022.
/// </summary>
public sealed class Session022Serializer
{
    public string Serialize(Session022InteractionContract state) =>
        JsonSerializer.Serialize(state);

    public Session022InteractionContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session022InteractionContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
