using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session032MissionRuntimeTests
{
    [Fact]
    public void TransitionEvaluator_SelectsFallbackWhenNoConditionsMatch()
    {
        var evaluator = new TransitionEvaluator();
        var transitions = new[]
        {
            new MissionTransition { TargetNodeId = "locked", Priority = 100, ConditionKey = "cond.locked" },
            new MissionTransition { TargetNodeId = "fallback", Priority = 1 }
        };

        var selected = evaluator.SelectNextTransition(transitions, Array.Empty<string>());

        Assert.NotNull(selected);
        Assert.Equal("fallback", selected!.TargetNodeId);
    }

    [Fact]
    public void MissionNode_TerminalFlag_CanBeSet()
    {
        var node = new MissionNode
        {
            NodeId = "end",
            Description = "Terminal node",
            IsTerminal = true,
            Transitions = Array.Empty<MissionTransition>()
        };

        Assert.True(node.IsTerminal);
        Assert.Empty(node.Transitions);
    }
}
