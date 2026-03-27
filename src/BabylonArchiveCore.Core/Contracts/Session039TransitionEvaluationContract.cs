namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S039: параметры оценки переходов узлов миссии.
/// </summary>
public sealed class Session039TransitionEvaluationContract
{
    public int ContractVersion { get; init; } = 39;

    public float PriorityWeight { get; init; } = 1f;

    public float ConditionSatisfiedBonus { get; init; } = 1000f;

    public float FallbackPenalty { get; init; } = 250f;

    public bool DeterministicTieBreak { get; init; } = true;
}
