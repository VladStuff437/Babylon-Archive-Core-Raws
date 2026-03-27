using BabylonArchiveCore.Core.Archive;

namespace BabylonArchiveCore.Runtime.Generation;

/// <summary>
/// Seed-based deterministic level generator.
/// </summary>
public sealed class LevelGenerator
{
    public LevelLayout Generate(ArchiveAddress address, IReadOnlyList<RoomArchetypeDefinition> archetypes, int roomCount)
    {
        ArgumentNullException.ThrowIfNull(archetypes);

        if (roomCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(roomCount), "roomCount must be greater than zero.");
        }

        if (archetypes.Count == 0)
        {
            throw new InvalidOperationException("At least one room archetype is required.");
        }

        var seed = ArchiveSeed.ToSeed(address);
        var random = new Random(seed);
        var rooms = new List<LevelRoom>(roomCount);

        for (var index = 0; index < roomCount; index++)
        {
            var archetype = PickArchetype(archetypes, random);
            rooms.Add(new LevelRoom
            {
                Index = index,
                ArchetypeId = archetype.ArchetypeId,
                LocalSeed = ArchiveSeed.DeriveChildSeed(seed, archetype.ArchetypeId, index)
            });
        }

        return new LevelLayout
        {
            Address = address,
            Seed = seed,
            Strategy = "weighted",
            Rooms = rooms
        };
    }

    public LevelLayout GenerateCore(ArchiveAddress address, IReadOnlyList<RoomArchetypeDefinition> archetypes, int roomCount, int worldSeed)
    {
        ArgumentNullException.ThrowIfNull(archetypes);

        if (roomCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(roomCount), "roomCount must be greater than zero.");
        }

        if (archetypes.Count == 0)
        {
            throw new InvalidOperationException("At least one room archetype is required.");
        }

        var seed = ArchiveSeed.DeriveHierarchySeed(address, worldSeed);
        var rooms = new List<LevelRoom>(roomCount);

        for (var depth = 0; depth < roomCount; depth++)
        {
            var pool = RoomArchetypeCatalog.SelectForDepth(archetypes, depth);
            var localSeed = ArchiveSeed.ComposeSeed(seed, new SeedScope("depth", depth));
            var random = new Random(localSeed);
            var archetype = PickArchetype(pool, random);

            rooms.Add(new LevelRoom
            {
                Index = depth,
                ArchetypeId = archetype.ArchetypeId,
                LocalSeed = localSeed
            });
        }

        return new LevelLayout
        {
            Address = address,
            Seed = seed,
            Strategy = "generator-core",
            Rooms = rooms
        };
    }

    private static RoomArchetypeDefinition PickArchetype(IReadOnlyList<RoomArchetypeDefinition> archetypes, Random random)
    {
        var totalWeight = archetypes.Sum(archetype => Math.Max(1, archetype.DifficultyWeight));
        var roll = random.Next(totalWeight);

        var cursor = 0;
        foreach (var archetype in archetypes)
        {
            cursor += Math.Max(1, archetype.DifficultyWeight);
            if (roll < cursor)
            {
                return archetype;
            }
        }

        return archetypes[0];
    }
}
