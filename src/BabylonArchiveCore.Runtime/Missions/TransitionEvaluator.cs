using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Balance;

namespace BabylonArchiveCore.Runtime.Missions;

/// <summary>
/// Evaluator переходов миссии с приоритетами.
/// </summary>
public sealed class TransitionEvaluator
{
    private static readonly TransitionScoringParameters DefaultScoring = new();

    public MissionNode? SelectNextNode(MissionDefinition definition, string currentNodeId, IReadOnlyCollection<string> activeConditionKeys)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(currentNodeId);
        ArgumentNullException.ThrowIfNull(activeConditionKeys);

        var current = definition.FindNode(currentNodeId);
        if (current is null || current.IsTerminal)
        {
            return null;
        }

        var transition = SelectNextTransition(current.Transitions, activeConditionKeys);
        if (transition is null)
        {
            return null;
        }

        return definition.FindNode(transition.TargetNodeId);
    }

    public MissionTransition? SelectNextTransition(IReadOnlyList<MissionTransition> transitions, IReadOnlyCollection<string> activeConditionKeys)
    {
        return SelectNextTransitionWithScoring(transitions, activeConditionKeys, DefaultScoring);
    }

    public MissionTransition? SelectNextTransitionWithBalance(
        IReadOnlyList<MissionTransition> transitions,
        IReadOnlyCollection<string> activeConditionKeys,
        BalanceTable balanceTable)
    {
        ArgumentNullException.ThrowIfNull(balanceTable);
        return SelectNextTransitionWithScoring(transitions, activeConditionKeys, TransitionScoringParameters.FromBalance(balanceTable));
    }

    public MissionTransition? SelectNextTransitionWithScoring(
        IReadOnlyList<MissionTransition> transitions,
        IReadOnlyCollection<string> activeConditionKeys,
        TransitionScoringParameters scoring)
    {
        ArgumentNullException.ThrowIfNull(transitions);
        ArgumentNullException.ThrowIfNull(activeConditionKeys);
        ArgumentNullException.ThrowIfNull(scoring);

        var satisfied = transitions
            .Where(t => IsSatisfied(t, activeConditionKeys))
            .ToArray();

        var candidates = satisfied.Length > 0
            ? satisfied
            : transitions.Where(t => t.IsFallback).ToArray();

        if (candidates.Length == 0)
        {
            return null;
        }

        return candidates
            .OrderByDescending(t => EvaluateTransitionScore(t, activeConditionKeys, scoring))
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.IsFallback)
            .ThenBy(t => t.TargetNodeId, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    public float EvaluateTransitionScore(
        MissionTransition transition,
        IReadOnlyCollection<string> activeConditionKeys,
        TransitionScoringParameters scoring)
    {
        ArgumentNullException.ThrowIfNull(transition);
        ArgumentNullException.ThrowIfNull(activeConditionKeys);
        ArgumentNullException.ThrowIfNull(scoring);

        var isSatisfied = IsSatisfied(transition, activeConditionKeys);
        var baseScore = transition.Priority * Math.Max(0f, scoring.PriorityWeight);
        var satisfiedBonus = isSatisfied ? Math.Max(0f, scoring.ConditionSatisfiedBonus) : 0f;
        var fallbackPenalty = transition.IsFallback ? Math.Max(0f, scoring.FallbackPenalty) : 0f;

        return baseScore + satisfiedBonus - fallbackPenalty;
    }

    private static bool IsSatisfied(MissionTransition transition, IReadOnlyCollection<string> activeConditionKeys)
    {
        return transition.ConditionKey is null || activeConditionKeys.Contains(transition.ConditionKey, StringComparer.Ordinal);
    }
}

public sealed class TransitionScoringParameters
{
    public float PriorityWeight { get; init; } = 1f;

    public float ConditionSatisfiedBonus { get; init; } = 1000f;

    public float FallbackPenalty { get; init; } = 250f;

    public static TransitionScoringParameters FromBalance(BalanceTable table)
    {
        ArgumentNullException.ThrowIfNull(table);

        return new TransitionScoringParameters
        {
            PriorityWeight = table.GetScalar("mission.priorityWeight", 1f),
            ConditionSatisfiedBonus = table.GetScalar("mission.satisfiedBonus", 1000f),
            FallbackPenalty = table.GetScalar("mission.fallbackPenalty", 250f)
        };
    }
}
