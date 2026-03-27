using System.Linq;
using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Runtime.Generation;
using Xunit;

namespace BabylonArchiveCore.Tests.Generation;

public class Session041SeedGoldenTests
{
    [Fact]
    public void LevelGenerator_SameAddress_ProducesStableLayout()
    {
        var generator = new LevelGenerator();
        var address = new ArchiveAddress(1, 0, 0, 0, 0, 1);
        var archetypes = new[]
        {
            new RoomArchetypeDefinition { ArchetypeId = "intro", BiomeTag = "hall", DifficultyWeight = 2 },
            new RoomArchetypeDefinition { ArchetypeId = "safe", BiomeTag = "sanctum", DifficultyWeight = 1 }
        };

        var first = generator.Generate(address, archetypes, roomCount: 5);
        var second = generator.Generate(address, archetypes, roomCount: 5);

        Assert.Equal(first.Seed, second.Seed);
        Assert.Equal(first.Rooms.Select(room => room.ArchetypeId), second.Rooms.Select(room => room.ArchetypeId));
        Assert.Equal(first.Rooms.Select(room => room.LocalSeed), second.Rooms.Select(room => room.LocalSeed));
    }
}
