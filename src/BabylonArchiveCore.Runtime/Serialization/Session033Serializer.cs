using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта лута S033.
/// </summary>
public sealed class Session033Serializer
{
    public string Serialize(Session033LootContract state) => JsonSerializer.Serialize(state);

    public Session033LootContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session033LootContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
