using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session024RuntimeTests
{
    [Fact]
    public void PlayerState_CharacterAttributes_AreClamped()
    {
        var state = new PlayerState();

        state.SetStrength(999);
        state.SetAgility(0);
        state.SetIntellect(-5);
        state.SetVitality(45);

        Assert.Equal(100, state.Strength);
        Assert.Equal(1, state.Agility);
        Assert.Equal(1, state.Intellect);
        Assert.Equal(45, state.Vitality);
    }

    [Fact]
    public void Migration024_Returns_Default_When_Null()
    {
        var migration = new Migration_024();
        var result = migration.Migrate(null);

        Assert.Equal("player-1", result.PlayerId);
        Assert.Equal(10, result.Strength);
    }

    [Fact]
    public void Session024Serializer_RoundTrip()
    {
        var serializer = new Session024Serializer();
        var payload = new Session024CharacterAttributesContract
        {
            PlayerId = "player-2",
            Strength = 15,
            Agility = 13,
            Intellect = 12,
            Vitality = 11,
            UnspentPoints = 2
        };

        var json = serializer.Serialize(payload);
        var restored = serializer.Deserialize(json);

        Assert.Equal("player-2", restored.PlayerId);
        Assert.Equal(15, restored.Strength);
        Assert.Equal(2, restored.UnspentPoints);
    }
}
