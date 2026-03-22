namespace BabylonArchiveCore.Domain.Archive;

/// <summary>
/// Six flat directions on a hex grid (flat-top axial coordinates).
/// Vertical transitions (Up/Down between tiers) are handled separately
/// via <see cref="ArchiveNode.ExitUp"/> and <see cref="ArchiveNode.ExitDown"/>.
/// </summary>
public enum HexDirection
{
    North = 0,
    NorthEast = 1,
    SouthEast = 2,
    South = 3,
    SouthWest = 4,
    NorthWest = 5,
}
