namespace BabylonArchiveCore.Domain.Archive;

/// <summary>
/// Hierarchical address within the Babylon Archive.
/// Archive → Sector → Hall → Module → Shelf → Cell → Tome → Page.
/// Deterministically maps to a seed for procedural generation.
/// </summary>
public readonly record struct ArchiveAddress(
    int Sector,
    int Hall,
    int Module,
    int Shelf,
    int Cell,
    int Tome,
    int Page)
{
    public string ToCanonical() =>
        $"S{Sector:D2}.H{Hall:D2}.M{Module:D2}.SH{Shelf:D2}.C{Cell:D2}.T{Tome:D3}.P{Page:D3}";

    /// <summary>
    /// Produces a deterministic integer seed from the world seed and all address components.
    /// Same address + same worldSeed = same result, always.
    /// </summary>
    public int ToSeed(int worldSeed)
    {
        unchecked
        {
            var hash = worldSeed;
            hash = hash * 31 + Sector;
            hash = hash * 31 + Hall;
            hash = hash * 31 + Module;
            hash = hash * 31 + Shelf;
            hash = hash * 31 + Cell;
            hash = hash * 31 + Tome;
            hash = hash * 31 + Page;
            return hash;
        }
    }

    /// <summary>
    /// Parses a canonical address string: S00.H00.M00.SH00.C00.T000.P000
    /// </summary>
    public static ArchiveAddress Parse(string canonical)
    {
        ArgumentNullException.ThrowIfNull(canonical);
        var parts = canonical.Split('.');
        if (parts.Length != 7)
            throw new FormatException($"Invalid archive address format: '{canonical}'");

        return new ArchiveAddress(
            Sector: int.Parse(parts[0].AsSpan(1)),
            Hall: int.Parse(parts[1].AsSpan(1)),
            Module: int.Parse(parts[2].AsSpan(1)),
            Shelf: int.Parse(parts[3].AsSpan(2)),
            Cell: int.Parse(parts[4].AsSpan(1)),
            Tome: int.Parse(parts[5].AsSpan(1)),
            Page: int.Parse(parts[6].AsSpan(1)));
    }
}
