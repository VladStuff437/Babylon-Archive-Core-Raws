namespace BabylonArchiveCore.Domain.Archive;

/// <summary>
/// A single room or junction inside the Hard-Archive fractal structure.
/// Positioned on a hex grid (axial Q/R) within a specific tier (vertical layer).
/// </summary>
public sealed class ArchiveNode
{
    public required int Id { get; init; }
    public required int Tier { get; init; }
    public required int HexQ { get; init; }
    public required int HexR { get; init; }
    public required ArchiveNodeType NodeType { get; init; }

    /// <summary>Flat hex exits: direction → neighbor node id.</summary>
    public Dictionary<HexDirection, int> Exits { get; } = new();

    /// <summary>Vertical exit upward (diagonal staircase to higher tier).</summary>
    public int? ExitUp { get; set; }

    /// <summary>Vertical exit downward (diagonal staircase to lower tier).</summary>
    public int? ExitDown { get; set; }

    /// <summary>Total number of exits (flat + vertical).</summary>
    public int ExitCount => Exits.Count
        + (ExitUp.HasValue ? 1 : 0)
        + (ExitDown.HasValue ? 1 : 0);
}
