using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session031MissionRuntimeTests
{
    [Fact]
    public void TransitionEvaluator_SelectsHighestPrioritySatisfiedTransition()
    {
        var evaluator = new TransitionEvaluator();
        var transitions = new[]
        {
            new MissionTransition { TargetNodeId = "node-a", Priority = 1, ConditionKey = "cond.a" },
            new MissionTransition { TargetNodeId = "node-b", Priority = 10, ConditionKey = "cond.b" },
            new MissionTransition { TargetNodeId = "node-c", Priority = 5 }
        };

        var selected = evaluator.SelectNextTransition(transitions, new[] { "cond.b" });

        Assert.NotNull(selected);
        Assert.Equal("node-b", selected!.TargetNodeId);
    }

    [Fact]
    public void MissionDefinition_FindNode_ReturnsNodeById()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-1",
            Title = "Test Mission",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "start",
                    Description = "Start",
                    IsTerminal = false,
                    Transitions = Array.Empty<MissionTransition>()
                }
            }
        };

        var node = definition.FindNode("start");

        Assert.NotNull(node);
        Assert.Equal("start", node!.NodeId);
    }
}
