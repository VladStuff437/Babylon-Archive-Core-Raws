using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session028RuntimeTests
{
    [Fact]
    public void AutoAttackController_Tick_RespectsInterval()
    {
        var controller = new AutoAttackController();
        controller.Start("enemy-1", 2);

        Assert.True(controller.Tick());
        Assert.False(controller.Tick());
        Assert.True(controller.Tick());
    }

    [Fact]
    public void AutoAttackController_Tick_Suppressed_ByMissionTransitionLock()
    {
        var controller = new AutoAttackController();
        controller.Start("enemy-1", 1);
        controller.BeginMissionTransitionLock();

        Assert.False(controller.Tick());

        controller.EndMissionTransitionLock();
        Assert.True(controller.Tick());
    }

    [Fact]
    public void Migration028_Returns_Default_When_Null()
    {
        var migration = new Migration_028();
        var state = migration.Migrate(null);

        Assert.Equal("player-1", state.ActorId);
        Assert.False(state.IsActive);
        Assert.Equal(1, state.AttackIntervalTicks);
    }

    [Fact]
    public void Session028Serializer_RoundTrip()
    {
        var serializer = new Session028Serializer();
        var payload = new Session028AutoAttackContract
        {
            ActorId = "player-1",
            TargetId = "enemy-7",
            IsActive = true,
            AttackIntervalTicks = 3,
            IsMissionTransitionLocked = false
        };

        var json = serializer.Serialize(payload);
        var restored = serializer.Deserialize(json);

        Assert.Equal("enemy-7", restored.TargetId);
        Assert.Equal(3, restored.AttackIntervalTicks);
    }
}
