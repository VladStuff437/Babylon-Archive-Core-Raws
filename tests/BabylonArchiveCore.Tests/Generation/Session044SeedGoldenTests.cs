using System.Linq;
using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Runtime.Generation;
using Xunit;

namespace BabylonArchiveCore.Tests.Generation;

public class Session044SeedGoldenTests
{
    [Fact]
    public void LevelGenerator_SameSeedAndArchetypes_ProducesSameLayout()
    {
        var generator = new LevelGenerator();
        var address = new ArchiveAddress(4, 0, 0, 0, 0, 1);
        var archetypes = new[]
        {
            new RoomArchetypeDefinition { ArchetypeId = "fallback", BiomeTag = "safe", DifficultyWeight = 1 },
            new RoomArchetypeDefinition { ArchetypeId = "combat", BiomeTag = "hall", DifficultyWeight = 2 }
        };

        var first = generator.Generate(address, archetypes, 5);
        var second = generator.Generate(address, archetypes, 5);

        Assert.Equal(first.Seed, second.Seed);
        Assert.Equal(first.Rooms.Select(r => r.ArchetypeId), second.Rooms.Select(r => r.ArchetypeId));
    }
}
