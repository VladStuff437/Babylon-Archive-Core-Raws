using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session039MissionRuntimeTests
{
    [Fact]
    public void TransitionEvaluator_SelectNextTransitionWithScoring_UsesDeterministicTargetTieBreak()
    {
        var evaluator = new TransitionEvaluator();
        var selected = evaluator.SelectNextTransitionWithScoring(
            new[]
            {
                new MissionTransition { TargetNodeId = "node-b", Priority = 5, IsFallback = false },
                new MissionTransition { TargetNodeId = "node-a", Priority = 5, IsFallback = false }
            },
            Array.Empty<string>(),
            new TransitionScoringParameters
            {
                PriorityWeight = 1f,
                ConditionSatisfiedBonus = 100f,
                FallbackPenalty = 10f
            });

        Assert.NotNull(selected);
        Assert.Equal("node-a", selected!.TargetNodeId);
    }

    [Fact]
    public void TransitionEvaluator_EvaluateTransitionScore_IsDeterministicForSameInput()
    {
        var evaluator = new TransitionEvaluator();
        var transition = new MissionTransition
        {
            TargetNodeId = "n1",
            Priority = 7,
            ConditionKey = "cond.ok",
            IsFallback = false
        };

        var scoring = new TransitionScoringParameters
        {
            PriorityWeight = 1.2f,
            ConditionSatisfiedBonus = 20f,
            FallbackPenalty = 3f
        };

        var first = evaluator.EvaluateTransitionScore(transition, new[] { "cond.ok" }, scoring);
        var second = evaluator.EvaluateTransitionScore(transition, new[] { "cond.ok" }, scoring);

        Assert.Equal(first, second);
    }
}
