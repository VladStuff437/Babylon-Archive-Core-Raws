namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S038: финализированная структура узла миссии и переходов.
/// </summary>
public sealed class Session038MissionNodeContract
{
    public required string NodeId { get; init; }

    public bool IsTerminal { get; init; }

    public bool IsCheckpoint { get; init; }

    public required TransitionContract[] Transitions { get; init; }
}

public sealed class TransitionContract
{
    public required string TargetNodeId { get; init; }

    public int Priority { get; init; }

    public string? ConditionKey { get; init; }

    public bool IsFallback { get; init; }
}
