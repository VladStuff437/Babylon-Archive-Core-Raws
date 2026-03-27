namespace BabylonArchiveCore.Runtime.Generation;

public sealed class RoomArchetypeDefinition
{
    public required string ArchetypeId { get; init; }

    public required string BiomeTag { get; init; }

    public int DifficultyWeight { get; init; } = 1;

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
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

    public required IReadOnlyList<LevelRoom> Rooms { get; init; }
}
