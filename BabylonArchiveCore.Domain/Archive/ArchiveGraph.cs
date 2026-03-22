namespace BabylonArchiveCore.Domain.Archive;

/// <summary>
/// A procedurally generated graph of <see cref="ArchiveNode"/> elements
/// representing one walkable area of the Hard-Archive.
/// </summary>
public sealed class ArchiveGraph
{
    public required int Seed { get; init; }
    public required int EntryNodeId { get; init; }
    public required int TierCount { get; init; }
    public Dictionary<int, ArchiveNode> Nodes { get; init; } = new();
}
