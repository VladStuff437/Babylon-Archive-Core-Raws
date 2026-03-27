using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта базового баланса S032.
/// </summary>
public sealed class Session032Serializer
{
    public string Serialize(Session032BalanceContract state) => JsonSerializer.Serialize(state);

    public Session032BalanceContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session032BalanceContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
