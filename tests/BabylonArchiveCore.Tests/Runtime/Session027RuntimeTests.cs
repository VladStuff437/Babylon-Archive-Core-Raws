using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.Input;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session027RuntimeTests
{
    [Fact]
    public void CombatInputHandler_TabEsc_Work_When_Unlocked()
    {
        var handler = new CombatInputHandler();
        handler.SetTargets(new[] { "e1", "e2" });

        Assert.Equal("e1", handler.CurrentTarget);
        handler.TabTarget();
        Assert.Equal("e2", handler.CurrentTarget);
        handler.EscCancel();
        Assert.Null(handler.CurrentTarget);
    }

    [Fact]
    public void CombatInputHandler_TabEsc_Blocked_DuringMissionTransition()
    {
        var handler = new CombatInputHandler();
        handler.SetTargets(new[] { "e1", "e2" });
        handler.BeginMissionTransitionLock();

        Assert.Equal("e1", handler.TabTarget());
        handler.EscCancel();
        Assert.Equal("e1", handler.CurrentTarget);

        handler.EndMissionTransitionLock();
        handler.EscCancel();
        Assert.Null(handler.CurrentTarget);
    }

    [Fact]
    public void Migration027_Returns_Default_When_Null()
    {
        var migration = new Migration_027();
        var state = migration.Migrate(null);

        Assert.Equal("player-1", state.ActorId);
        Assert.Empty(state.TargetQueue);
        Assert.False(state.IsMissionTransitionLocked);
    }

    [Fact]
    public void Session027Serializer_RoundTrip()
    {
        var serializer = new Session027Serializer();
        var payload = new Session027CombatInputContract
        {
            ActorId = "player-1",
            TargetQueue = new[] { "enemy-1", "enemy-2" },
            CurrentTargetId = "enemy-1",
            IsTargetingEnabled = true,
            IsMissionTransitionLocked = false
        };

        var json = serializer.Serialize(payload);
        var restored = serializer.Deserialize(json);

        Assert.Equal("player-1", restored.ActorId);
        Assert.Equal(2, restored.TargetQueue.Length);
    }
}
