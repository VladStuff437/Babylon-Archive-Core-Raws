using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта статус-системы S025.
/// </summary>
public sealed class Session025Serializer
{
    public string Serialize(Session025StatusSystemContract state) => JsonSerializer.Serialize(state);

    public Session025StatusSystemContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session025StatusSystemContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
