namespace BabylonArchiveCore.Core.Archive;

/// <summary>
/// Canonical archive address: Sector/Hall/Module/Shelf/Tome/Page.
/// </summary>
public readonly record struct ArchiveAddress(
    int Sector,
    int Hall,
    int Module,
    int Shelf,
    int Tome,
    int Page)
{
    public bool IsRoot => Sector == 0 && Hall == 0 && Module == 0 && Shelf == 0 && Tome == 0 && Page == 0;

    public IReadOnlyList<int> ToPath() => new[] { Sector, Hall, Module, Shelf, Tome, Page };

    public ArchiveAddress NextPage() => new(Sector, Hall, Module, Shelf, Tome, Page + 1);

    public ArchiveAddress Parent()
    {
        if (Page > 0) return new ArchiveAddress(Sector, Hall, Module, Shelf, Tome, Page - 1);
        if (Tome > 0) return new ArchiveAddress(Sector, Hall, Module, Shelf, Tome - 1, 0);
        if (Shelf > 0) return new ArchiveAddress(Sector, Hall, Module, Shelf - 1, 0, 0);
        if (Module > 0) return new ArchiveAddress(Sector, Hall, Module - 1, 0, 0, 0);
        if (Hall > 0) return new ArchiveAddress(Sector, Hall - 1, 0, 0, 0, 0);
        if (Sector > 0) return new ArchiveAddress(Sector - 1, 0, 0, 0, 0, 0);
        return this;
    }

    public static ArchiveAddress Parse(string value)
    {
        if (!TryParse(value, out var address))
        {
            throw new FormatException($"Invalid archive address '{value}'.");
        }

        return address;
    }

    public static bool TryParse(string? value, out ArchiveAddress address)
    {
        address = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 6)
        {
            return false;
        }

        if (!TryReadPart(parts[0], "S", out var sector) ||
            !TryReadPart(parts[1], "H", out var hall) ||
            !TryReadPart(parts[2], "M", out var module) ||
            !TryReadPart(parts[3], "Sh", out var shelf) ||
            !TryReadPart(parts[4], "T", out var tome) ||
            !TryReadPart(parts[5], "P", out var page))
        {
            return false;
        }

        address = new ArchiveAddress(sector, hall, module, shelf, tome, page);
        return true;
    }

    public string ToCanonicalString()
    {
        return $"S{Sector}:H{Hall}:M{Module}:Sh{Shelf}:T{Tome}:P{Page}";
    }

    public override string ToString() => ToCanonicalString();

    private static bool TryReadPart(string part, string prefix, out int value)
    {
        value = 0;
        if (!part.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        return int.TryParse(part[prefix.Length..], out value) && value >= 0;
    }
}
