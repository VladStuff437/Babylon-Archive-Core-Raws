using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Runtime.Generation;
using Xunit;

namespace BabylonArchiveCore.Tests.Generation;

public class Session042SeedGoldenTests
{
    [Fact]
    public void LevelGenerator_DifferentAddresses_ProduceDifferentSeed()
    {
        var generator = new LevelGenerator();
        var archetypes = new[]
        {
            new RoomArchetypeDefinition { ArchetypeId = "combat", BiomeTag = "hall", DifficultyWeight = 3 }
        };

        var left = generator.Generate(new ArchiveAddress(1, 1, 1, 1, 1, 1), archetypes, roomCount: 4);
        var right = generator.Generate(new ArchiveAddress(1, 1, 1, 1, 1, 2), archetypes, roomCount: 4);

        Assert.NotEqual(left.Seed, right.Seed);
        Assert.Equal(4, left.Rooms.Count);
        Assert.Equal(4, right.Rooms.Count);
    }
}
