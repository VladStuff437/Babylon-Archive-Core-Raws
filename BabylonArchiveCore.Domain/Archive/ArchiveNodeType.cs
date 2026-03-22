namespace BabylonArchiveCore.Domain.Archive;

public enum ArchiveNodeType
{
    /// <summary>Hexagonal junction — up to 6 flat exits.</summary>
    Crossroads,

    /// <summary>Linear passage — exactly 2 exits.</summary>
    Corridor,

    /// <summary>Contains shelves / cells / tomes — 1-3 exits.</summary>
    ShelfRoom,

    /// <summary>Diagonal tier connector — flat exit + vertical exit.</summary>
    Staircase,

    /// <summary>Terminal access point — 1-2 exits.</summary>
    TerminalRoom,
}
