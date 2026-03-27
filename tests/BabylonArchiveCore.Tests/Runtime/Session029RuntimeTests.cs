using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.AI.Perception;
using BabylonArchiveCore.Runtime.AI.StateMachine;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session029RuntimeTests
{
    [Fact]
    public void PerceptionSystem_DetectTargetsWithAwareness_RespectsThresholdAndRadius()
    {
        var perception = new PerceptionSystem
        {
            DetectionRadius = 8f,
            AlertThreshold = 0.5f
        };

        var targets = perception.DetectTargetsWithAwareness(0f, new[]
        {
            ("enemy-1", 4f, 0.6f),
            ("enemy-2", 12f, 0.9f),
            ("enemy-3", 3f, 0.2f)
        });

        Assert.Single(targets);
        Assert.Equal("enemy-1", targets[0]);
    }

    [Fact]
    public void PerceptionSystem_SelectPrimaryTarget_PrefersPreviousIfStillVisible()
    {
        var perception = new PerceptionSystem();
        var selected = perception.SelectPrimaryTarget(new[] { "enemy-1", "enemy-2" }, "enemy-2");

        Assert.Equal("enemy-2", selected);
    }

    [Fact]
    public void AIStateMachine_UpdateFromPerception_Transitions()
    {
        var machine = new AIStateMachine();

        machine.UpdateFromPerception(true, 1, 1.5f, 100f);
        Assert.Equal(AIState.Attack, machine.Current);

        machine.UpdateFromPerception(false, 0, 99f, 100f);
        Assert.Equal(AIState.Patrol, machine.Current);
    }

    [Fact]
    public void Migration029_Returns_Default_When_Null()
    {
        var migration = new Migration_029();
        var state = migration.Migrate(null);

        Assert.Equal("enemy-1", state.AgentId);
        Assert.Empty(state.VisibleTargets);
    }

    [Fact]
    public void Session029Serializer_RoundTrip()
    {
        var serializer = new Session029Serializer();
        var payload = new Session029PerceptionContract
        {
            AgentId = "enemy-9",
            DetectionRadius = 14f,
            AlertThreshold = 0.6f,
            VisibleTargets = new[] { "player-1" },
            PrimaryTargetId = "player-1"
        };

        var json = serializer.Serialize(payload);
        var restored = serializer.Deserialize(json);

        Assert.Equal("enemy-9", restored.AgentId);
        Assert.Equal("player-1", restored.PrimaryTargetId);
    }
}
