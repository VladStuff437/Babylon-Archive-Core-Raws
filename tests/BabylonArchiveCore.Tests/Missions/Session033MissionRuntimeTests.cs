using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session033MissionRuntimeTests
{
    [Fact]
    public void MissionDefinition_Validate_FindsMissingStartNode()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-033",
            Title = "Loot Mission",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "a",
                    Description = "A",
                    IsTerminal = false,
                    Transitions = Array.Empty<MissionTransition>()
                }
            }
        };

        var errors = definition.Validate();

        Assert.Contains(errors, e => e.Contains("Start node", StringComparison.Ordinal));
    }

    [Fact]
    public void MissionNode_ValidateTransitions_FindsUnknownTarget()
    {
        var node = new MissionNode
        {
            NodeId = "a",
            Description = "A",
            IsTerminal = false,
            Transitions = new[]
            {
                new MissionTransition { TargetNodeId = "missing", Priority = 1 }
            }
        };

        var errors = node.ValidateTransitions(new[] { "a", "b" });

        Assert.Single(errors);
    }

    [Fact]
    public void TransitionEvaluator_SelectNextNode_UsesPriority()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-033",
            Title = "Loot Mission",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "start",
                    Description = "Start",
                    IsTerminal = false,
                    Transitions = new[]
                    {
                        new MissionTransition { TargetNodeId = "low", Priority = 1, ConditionKey = "ok" },
                        new MissionTransition { TargetNodeId = "high", Priority = 10, ConditionKey = "ok" }
                    }
                },
                new MissionNode { NodeId = "low", Description = "Low", IsTerminal = true, Transitions = Array.Empty<MissionTransition>() },
                new MissionNode { NodeId = "high", Description = "High", IsTerminal = true, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var evaluator = new TransitionEvaluator();
        var next = evaluator.SelectNextNode(definition, "start", new[] { "ok" });

        Assert.NotNull(next);
        Assert.Equal("high", next!.NodeId);
    }
}
