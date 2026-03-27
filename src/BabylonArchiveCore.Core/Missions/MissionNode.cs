namespace BabylonArchiveCore.Core.Missions;

/// <summary>
/// Узел миссии с переходами и terminal-флагом.
/// </summary>
public sealed class MissionNode
{
    public required string NodeId { get; init; }

    public required string Description { get; init; }

    public bool IsTerminal { get; init; }

    public bool IsCheckpoint { get; init; }

    public required IReadOnlyList<MissionTransition> Transitions { get; init; }

    public IReadOnlyList<string> ValidateTransitions(IReadOnlyCollection<string> knownNodeIds)
    {
        ArgumentNullException.ThrowIfNull(knownNodeIds);

        var errors = new List<string>();
        foreach (var transition in Transitions)
        {
            if (!knownNodeIds.Contains(transition.TargetNodeId, StringComparer.Ordinal))
            {
                errors.Add($"Node '{NodeId}' references unknown target '{transition.TargetNodeId}'.");
            }
        }

        var duplicateTargets = Transitions
            .GroupBy(t => t.TargetNodeId, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        foreach (var duplicateTarget in duplicateTargets)
        {
            errors.Add($"Node '{NodeId}' has duplicate transition target '{duplicateTarget}'.");
        }

        var fallbackCount = Transitions.Count(t => t.IsFallback);
        if (fallbackCount > 1)
        {
            errors.Add($"Node '{NodeId}' cannot define more than one fallback transition.");
        }

        if (IsTerminal && Transitions.Count > 0)
        {
            errors.Add($"Terminal node '{NodeId}' cannot define transitions.");
        }

        return errors;
    }
}

public sealed class MissionTransition
{
    public required string TargetNodeId { get; init; }

    public int Priority { get; init; }

    public string? ConditionKey { get; init; }

    public bool IsFallback { get; init; }
}
