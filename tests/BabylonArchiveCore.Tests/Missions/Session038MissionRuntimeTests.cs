using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session038MissionRuntimeTests
{
    [Fact]
    public void TransitionEvaluator_PrefersNonFallback_WhenConditionsSatisfied()
    {
        var evaluator = new TransitionEvaluator();
        var selected = evaluator.SelectNextTransition(
            new[]
            {
                new MissionTransition { TargetNodeId = "fallback", Priority = 10, IsFallback = true },
                new MissionTransition { TargetNodeId = "normal", Priority = 10, IsFallback = false }
            },
            Array.Empty<string>());

        Assert.NotNull(selected);
        Assert.Equal("normal", selected!.TargetNodeId);
    }
}
