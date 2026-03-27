using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта формул урона S031.
/// </summary>
public sealed class Session031Serializer
{
    public string Serialize(Session031DamageFormulaContract state) => JsonSerializer.Serialize(state);

    public Session031DamageFormulaContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session031DamageFormulaContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
