using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S039: параметры оценки переходов миссий.
/// </summary>
public sealed class Migration_039
{
    public Session039TransitionEvaluationContract Migrate(object? legacyState)
    {
        if (legacyState is Session039TransitionEvaluationContract existing)
        {
            return existing;
        }

        return new Session039TransitionEvaluationContract
        {
            ContractVersion = 39,
            PriorityWeight = 1f,
            ConditionSatisfiedBonus = 1000f,
            FallbackPenalty = 250f,
            DeterministicTieBreak = true
        };
    }
}
