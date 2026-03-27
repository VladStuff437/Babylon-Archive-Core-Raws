using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session037CombatSmokeTests
{
    [Fact]
    public void MissionDefinitionContractFlow_Smoke()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-037",
            Title = "Contract Flow",
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
                        new MissionTransition { TargetNodeId = "end", Priority = 1, IsFallback = true }
                    }
                },
                new MissionNode { NodeId = "end", Description = "End", IsTerminal = true, IsCheckpoint = false, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var evaluator = new TransitionEvaluator();
        var next = evaluator.SelectNextNode(definition, "start", Array.Empty<string>());

        Assert.NotNull(next);
        Assert.Equal("end", next!.NodeId);
    }
}
