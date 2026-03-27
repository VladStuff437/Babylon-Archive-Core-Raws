using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session038CombatSmokeTests
{
    [Fact]
    public void MissionNodeFallbackFlow_Smoke()
    {
        var evaluator = new TransitionEvaluator();
        var transitions = new[]
        {
            new MissionTransition { TargetNodeId = "locked", Priority = 5, ConditionKey = "need.key", IsFallback = false },
            new MissionTransition { TargetNodeId = "safe", Priority = 1, ConditionKey = "need.other", IsFallback = true }
        };

        var next = evaluator.SelectNextTransition(transitions, Array.Empty<string>());
        Assert.NotNull(next);
        Assert.Equal("safe", next!.TargetNodeId);
    }
}
