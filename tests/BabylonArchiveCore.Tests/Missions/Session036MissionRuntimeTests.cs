using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session036MissionRuntimeTests
{
    [Fact]
    public void TransitionEvaluator_SelectsHighestPrioritySatisfiedTransition()
    {
        var evaluator = new TransitionEvaluator();
        var definition = new MissionDefinition
        {
            MissionId = "mission-036",
            Title = "Axes Mission",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "start",
                    Description = "Start",
                    IsTerminal = false,
                    IsCheckpoint = true,
                    Transitions = new[]
                    {
                        new MissionTransition { TargetNodeId = "a", Priority = 1, ConditionKey = "ok", IsFallback = false },
                        new MissionTransition { TargetNodeId = "b", Priority = 5, ConditionKey = "ok", IsFallback = false }
                    }
                },
                new MissionNode { NodeId = "a", Description = "A", IsTerminal = true, IsCheckpoint = false, Transitions = Array.Empty<MissionTransition>() },
                new MissionNode { NodeId = "b", Description = "B", IsTerminal = true, IsCheckpoint = false, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var next = evaluator.SelectNextNode(definition, "start", new[] { "ok" });
        Assert.NotNull(next);
        Assert.Equal("b", next!.NodeId);
    }
}
