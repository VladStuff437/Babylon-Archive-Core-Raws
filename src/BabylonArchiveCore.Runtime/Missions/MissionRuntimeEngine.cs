using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Balance;

namespace BabylonArchiveCore.Runtime.Missions;

/// <summary>
/// Runtime orchestration for deterministic mission graph progression.
/// </summary>
public sealed class MissionRuntimeEngine
{
    private readonly TransitionEvaluator transitionEvaluator = new();

    public MissionRuntimeState Start(MissionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var start = definition.FindNode(definition.StartNodeId)
            ?? throw new InvalidOperationException($"Start node '{definition.StartNodeId}' is missing.");

        return new MissionRuntimeState(start.NodeId, start.IsTerminal);
    }

    public MissionNode? Advance(
        MissionDefinition definition,
        MissionRuntimeState state,
        IReadOnlyCollection<string> activeConditionKeys,
        BalanceTable? balanceTable = null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(activeConditionKeys);

        if (state.IsCompleted)
        {
            return null;
        }

        var current = definition.FindNode(state.CurrentNodeId)
            ?? throw new InvalidOperationException($"Current node '{state.CurrentNodeId}' is missing.");

        if (current.IsTerminal)
        {
            state.MarkCompleted();
            return null;
        }

        var nextTransition = balanceTable is null
            ? transitionEvaluator.SelectNextTransition(current.Transitions, activeConditionKeys)
            : transitionEvaluator.SelectNextTransitionWithBalance(current.Transitions, activeConditionKeys, balanceTable);

        if (nextTransition is null)
        {
            return null;
        }

        var nextNode = definition.FindNode(nextTransition.TargetNodeId)
            ?? throw new InvalidOperationException($"Target node '{nextTransition.TargetNodeId}' is missing.");

        state.MoveTo(nextNode.NodeId, nextNode.IsTerminal);
        return nextNode;
    }
}

public sealed class MissionRuntimeState
{
    private readonly HashSet<string> visitedNodeIds = new(StringComparer.Ordinal);

    public MissionRuntimeState(string startNodeId, bool isCompleted)
        : this(startNodeId, isCompleted, 0, new[] { startNodeId })
    {
    }

    private MissionRuntimeState(string currentNodeId, bool isCompleted, int stepCount, IReadOnlyCollection<string> visited)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentNodeId);
        ArgumentNullException.ThrowIfNull(visited);

        CurrentNodeId = currentNodeId;
        IsCompleted = isCompleted;
        StepCount = Math.Max(0, stepCount);

        foreach (var nodeId in visited.Where(nodeId => !string.IsNullOrWhiteSpace(nodeId)))
        {
            visitedNodeIds.Add(nodeId);
        }

        if (visitedNodeIds.Count == 0)
        {
            visitedNodeIds.Add(currentNodeId);
        }
    }

    public string CurrentNodeId { get; private set; }

    public bool IsCompleted { get; private set; }

    public int StepCount { get; private set; }

    public IReadOnlyCollection<string> VisitedNodeIds => visitedNodeIds;

    public static MissionRuntimeState Restore(string currentNodeId, bool isCompleted, int stepCount, IReadOnlyCollection<string> visitedNodeIds)
    {
        return new MissionRuntimeState(currentNodeId, isCompleted, stepCount, visitedNodeIds);
    }

    public void MoveTo(string nodeId, bool isTerminal)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);

        CurrentNodeId = nodeId;
        IsCompleted = isTerminal;
        StepCount++;
        visitedNodeIds.Add(nodeId);
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
    }
}
