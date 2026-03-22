namespace BabylonArchiveCore.Domain.Mission;

/// <summary>
/// A mission definition: metadata + directed graph of nodes + consequence effects.
/// </summary>
public sealed class MissionDefinition
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required MissionType Type { get; init; }
    public MissionTerminalTab TerminalTab { get; init; } = MissionTerminalTab.Core;
    public bool RequiresProtocolZero { get; init; }
    public bool IsReplayable { get; init; } = true;
    public required string StartNodeId { get; init; }

    /// <summary>Optional time limit in seconds (for Timed missions).</summary>
    public int? TimeLimitSeconds { get; init; }

    /// <summary>All nodes in this mission graph, keyed by node id.</summary>
    public Dictionary<string, MissionNode> Nodes { get; init; } = new();

    /// <summary>Effects applied on successful completion.</summary>
    public MissionEffect? OnSuccess { get; init; }

    /// <summary>Effects applied on failure.</summary>
    public MissionEffect? OnFailure { get; init; }
}
