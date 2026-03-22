namespace BabylonArchiveCore.Domain.Mission;

/// <summary>
/// A single step (node) inside a mission graph.
/// Edges are encoded as <see cref="Transitions"/>: condition label → target node id.
/// </summary>
public sealed class MissionNode
{
    public required string Id { get; init; }
    public required string Description { get; init; }

    /// <summary>If true, reaching this node completes the mission successfully.</summary>
    public bool IsTerminalSuccess { get; init; }

    /// <summary>If true, reaching this node fails the mission.</summary>
    public bool IsTerminalFailure { get; init; }

    /// <summary>Outgoing edges: condition label → target node id.</summary>
    public Dictionary<string, string> Transitions { get; init; } = new();
}
