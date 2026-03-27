using BabylonArchiveCore.Core.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session035MissionRuntimeTests
{
    [Fact]
    public void MissionDefinition_Validate_DetectsDuplicateNodeIds()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-035",
            Title = "Faction Mission",
            StartNodeId = "node",
            Nodes = new[]
            {
                new MissionNode { NodeId = "node", Description = "A", IsTerminal = false, Transitions = Array.Empty<MissionTransition>() },
                new MissionNode { NodeId = "node", Description = "B", IsTerminal = true, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var errors = definition.Validate();

        Assert.Contains(errors, e => e.Contains("Duplicate node id", StringComparison.Ordinal));
    }

    [Fact]
    public void MissionNode_ValidateTransitions_RejectsTerminalWithTransitions()
    {
        var node = new MissionNode
        {
            NodeId = "terminal",
            Description = "Terminal node",
            IsTerminal = true,
            Transitions = new[]
            {
                new MissionTransition { TargetNodeId = "other", Priority = 1 }
            }
        };

        var errors = node.ValidateTransitions(new[] { "terminal", "other" });

        Assert.Contains(errors, e => e.Contains("Terminal node", StringComparison.Ordinal));
    }
}
