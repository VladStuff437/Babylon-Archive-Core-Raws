namespace BabylonArchiveCore.Runtime.Generation;

public sealed class RoomArchetypeDefinition
{
    public required string ArchetypeId { get; init; }

    public required string BiomeTag { get; init; }

    public int DifficultyWeight { get; init; } = 1;

    public int MinDepth { get; init; } = 0;

    public int MaxDepth { get; init; } = int.MaxValue;

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    public bool SupportsDepth(int depth) => depth >= MinDepth && depth <= MaxDepth;
}

public sealed class LevelRoom
{
    public required string ArchetypeId { get; init; }

    public int LocalSeed { get; init; }

    public int Index { get; init; }
}

public sealed class LevelLayout
{
    public required Core.Archive.ArchiveAddress Address { get; init; }

    public int Seed { get; init; }

    public string Strategy { get; init; } = "weighted";

    public required IReadOnlyList<LevelRoom> Rooms { get; init; }
}
