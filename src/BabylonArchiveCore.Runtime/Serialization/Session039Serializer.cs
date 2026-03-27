using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта оценки переходов S039.
/// </summary>
public sealed class Session039Serializer
{
    public string Serialize(Session039TransitionEvaluationContract state) => JsonSerializer.Serialize(state);

    public Session039TransitionEvaluationContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session039TransitionEvaluationContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
