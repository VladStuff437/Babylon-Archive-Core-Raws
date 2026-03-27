using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session034MissionRuntimeTests
{
    [Fact]
    public void TransitionEvaluator_SelectNextTransition_TieBreaksByTargetId()
    {
        var evaluator = new TransitionEvaluator();
        var selected = evaluator.SelectNextTransition(
            new[]
            {
                new MissionTransition { TargetNodeId = "node-b", Priority = 5 },
                new MissionTransition { TargetNodeId = "node-a", Priority = 5 }
            },
            Array.Empty<string>());

        Assert.NotNull(selected);
        Assert.Equal("node-a", selected!.TargetNodeId);
    }

    [Fact]
    public void TransitionEvaluator_SelectNextNode_ReturnsNullForTerminalNode()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-034",
            Title = "Economy Mission",
            StartNodeId = "end",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "end",
                    Description = "Terminal",
                    IsTerminal = true,
                    Transitions = Array.Empty<MissionTransition>()
                }
            }
        };

        var evaluator = new TransitionEvaluator();
        var next = evaluator.SelectNextNode(definition, "end", Array.Empty<string>());

        Assert.Null(next);
    }
}
