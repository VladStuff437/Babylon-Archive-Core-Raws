using BabylonArchiveCore.Runtime.AI.Perception;
using BabylonArchiveCore.Runtime.AI.StateMachine;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session030CombatSmokeTests
{
    [Fact]
    public void EnemyStateMachine_WithPerceptionSnapshot_Smoke()
    {
        var perception = new PerceptionSystem
        {
            DetectionRadius = 12f,
            AlertThreshold = 0.4f
        };

        var visible = perception.DetectTargetsWithAwareness(0f, new[]
        {
            ("player-1", 5f, 0.9f),
            ("player-2", 15f, 0.9f)
        });

        var machine = new AIStateMachine();
        machine.UpdateEnemyState(new EnemyStateInput
        {
            HasLineOfSight = visible.Count > 0,
            DistanceToTarget = 1.8f,
            Aggression = 0.7f,
            Health = 90f
        });

        Assert.Equal(AIState.Attack, machine.Current);
    }
}
