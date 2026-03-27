using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта экономики S034.
/// </summary>
public sealed class Session034Serializer
{
    public string Serialize(Session034EconomyContract state) => JsonSerializer.Serialize(state);

    public Session034EconomyContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session034EconomyContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
