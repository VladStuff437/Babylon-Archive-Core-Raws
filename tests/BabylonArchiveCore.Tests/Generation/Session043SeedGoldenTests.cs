using System.Linq;
using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Runtime.Generation;
using Xunit;

namespace BabylonArchiveCore.Tests.Generation;

public class Session043SeedGoldenTests
{
    [Fact]
    public void ArchiveSeed_DeriveChildSeed_IsStableHierarchy()
    {
        var root = ArchiveSeed.ToSeed(new ArchiveAddress(3, 2, 1, 0, 0, 7), worldSeed: 11);
        var childA = ArchiveSeed.DeriveChildSeed(root, "intro-combat-hall", 0);
        var childB = ArchiveSeed.DeriveChildSeed(root, "intro-combat-hall", 0);
        var sibling = ArchiveSeed.DeriveChildSeed(root, "intro-combat-hall", 1);

        Assert.Equal(childA, childB);
        Assert.NotEqual(childA, sibling);
    }

    [Fact]
    public void LevelGenerator_WeightedArchetypes_StillDeterministic()
    {
        var generator = new LevelGenerator();
        var address = new ArchiveAddress(2, 2, 2, 2, 2, 2);
        var archetypes = new[]
        {
            new RoomArchetypeDefinition { ArchetypeId = "combat", BiomeTag = "hall", DifficultyWeight = 5 },
            new RoomArchetypeDefinition { ArchetypeId = "puzzle", BiomeTag = "core", DifficultyWeight = 2 },
            new RoomArchetypeDefinition { ArchetypeId = "safe", BiomeTag = "sanctum", DifficultyWeight = 1 }
        };

        var first = generator.Generate(address, archetypes, roomCount: 6);
        var second = generator.Generate(address, archetypes, roomCount: 6);

        Assert.Equal(first.Rooms.Select(room => room.ArchetypeId), second.Rooms.Select(room => room.ArchetypeId));
    }
}
