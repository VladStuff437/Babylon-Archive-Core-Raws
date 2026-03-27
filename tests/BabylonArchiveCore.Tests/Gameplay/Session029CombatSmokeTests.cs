using BabylonArchiveCore.Runtime.AI.Perception;
using BabylonArchiveCore.Runtime.AI.StateMachine;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session029CombatSmokeTests
{
    [Fact]
    public void EnemyPerceptionAndStateMachine_Smoke()
    {
        var perception = new PerceptionSystem
        {
            DetectionRadius = 10f,
            AlertThreshold = 0.5f
        };

        var visible = perception.DetectTargetsWithAwareness(0f, new[]
        {
            ("player-1", 6f, 0.8f),
            ("player-2", 11f, 0.9f)
        });

        var primary = perception.SelectPrimaryTarget(visible, null);
        Assert.Equal("player-1", primary);

        var stateMachine = new AIStateMachine();
        stateMachine.UpdateFromPerception(true, visible.Count, 1.8f, 75f);

        Assert.Equal(AIState.Attack, stateMachine.Current);
    }
}
