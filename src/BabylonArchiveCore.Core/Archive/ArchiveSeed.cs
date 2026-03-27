namespace BabylonArchiveCore.Core.Archive;

/// <summary>
/// Deterministic seed derivation for archive hierarchy.
/// </summary>
public static class ArchiveSeed
{
    public static int ToSeed(ArchiveAddress address, int worldSeed = 0)
    {
        var hash = FnvOffset;
        hash = AppendInt(hash, worldSeed);
        hash = AppendInt(hash, address.Sector);
        hash = AppendInt(hash, address.Hall);
        hash = AppendInt(hash, address.Module);
        hash = AppendInt(hash, address.Shelf);
        hash = AppendInt(hash, address.Tome);
        hash = AppendInt(hash, address.Page);
        return Normalize(hash);
    }

    public static int DeriveChildSeed(int parentSeed, string scope, int index = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);

        var hash = FnvOffset;
        hash = AppendInt(hash, parentSeed);
        hash = AppendString(hash, scope);
        hash = AppendInt(hash, index);
        return Normalize(hash);
    }

    public static int DeriveHierarchySeed(ArchiveAddress address, int worldSeed = 0)
    {
        var seed = DeriveChildSeed(worldSeed, "sector", address.Sector);
        seed = DeriveChildSeed(seed, "hall", address.Hall);
        seed = DeriveChildSeed(seed, "module", address.Module);
        seed = DeriveChildSeed(seed, "shelf", address.Shelf);
        seed = DeriveChildSeed(seed, "tome", address.Tome);
        return DeriveChildSeed(seed, "page", address.Page);
    }

    public static int ComposeSeed(int baseSeed, params SeedScope[] scopes)
    {
        ArgumentNullException.ThrowIfNull(scopes);

        var seed = Normalize((uint)baseSeed);
        foreach (var scope in scopes)
        {
            seed = DeriveChildSeed(seed, scope.Scope, scope.Index);
        }

        return seed;
    }

    private const uint FnvOffset = 2166136261;
    private const uint FnvPrime = 16777619;

    private static uint AppendInt(uint hash, int value)
    {
        unchecked
        {
            hash ^= (uint)value;
            hash *= FnvPrime;
            return hash;
        }
    }

    private static uint AppendString(uint hash, string value)
    {
        unchecked
        {
            foreach (var ch in value)
            {
                hash ^= ch;
                hash *= FnvPrime;
            }

            return hash;
        }
    }

    private static int Normalize(uint hash)
    {
        var value = (int)(hash & 0x7FFFFFFF);
        return value == 0 ? 1 : value;
    }
}

public readonly record struct SeedScope(string Scope, int Index);
