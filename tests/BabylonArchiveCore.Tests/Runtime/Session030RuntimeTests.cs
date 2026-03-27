using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.AI.StateMachine;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session030RuntimeTests
{
    [Fact]
    public void AIStateMachine_UpdateEnemyState_Attack_WhenAggressiveAndNear()
    {
        var machine = new AIStateMachine();

        machine.UpdateEnemyState(new EnemyStateInput
        {
            HasLineOfSight = true,
            DistanceToTarget = 1.5f,
            Aggression = 0.8f,
            Health = 100f
        });

        Assert.Equal(AIState.Attack, machine.Current);
    }

    [Fact]
    public void AIStateMachine_UpdateEnemyState_Investigate_WhenLostSightRecently()
    {
        var machine = new AIStateMachine();

        machine.UpdateEnemyState(new EnemyStateInput
        {
            HasLineOfSight = false,
            LastSeenTargetTicks = 2,
            InvestigationTicks = 3,
            Health = 100f
        });

        Assert.Equal(AIState.Investigate, machine.Current);
    }

    [Fact]
    public void Migration030_ReturnsDefault_WhenNull()
    {
        var migration = new Migration_030();
        var state = migration.Migrate(null);

        Assert.Equal("enemy-1", state.AgentId);
        Assert.Equal("Idle", state.CurrentState);
    }

    [Fact]
    public void Session030Serializer_RoundTrip()
    {
        var serializer = new Session030Serializer();
        var payload = new Session030EnemyStateMachineContract
        {
            AgentId = "enemy-77",
            CurrentState = "Alert",
            Aggression = 0.65f,
            LeashDistance = 20f,
            LastSeenTargetTicks = 1,
            PrimaryTargetId = "player-1"
        };

        var json = serializer.Serialize(payload);
        var restored = serializer.Deserialize(json);

        Assert.Equal("enemy-77", restored.AgentId);
        Assert.Equal("Alert", restored.CurrentState);
    }
}
