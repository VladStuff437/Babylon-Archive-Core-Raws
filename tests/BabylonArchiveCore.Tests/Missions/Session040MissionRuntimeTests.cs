using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session040MissionRuntimeTests
{
    [Fact]
    public void MissionRuntimeState_TracksVisitedNodesAcrossSteps()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-040",
            Title = "Visited Nodes",
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
                        new MissionTransition { TargetNodeId = "mid", Priority = 10, IsFallback = true }
                    }
                },
                new MissionNode
                {
                    NodeId = "mid",
                    Description = "Mid",
                    IsTerminal = false,
                    IsCheckpoint = false,
                    Transitions = new[]
                    {
                        new MissionTransition { TargetNodeId = "end", Priority = 10, IsFallback = true }
                    }
                },
                new MissionNode
                {
                    NodeId = "end",
                    Description = "End",
                    IsTerminal = true,
                    IsCheckpoint = false,
                    Transitions = Array.Empty<MissionTransition>()
                }
            }
        };

        var engine = new MissionRuntimeEngine();
        var state = engine.Start(definition);

        engine.Advance(definition, state, Array.Empty<string>());
        engine.Advance(definition, state, Array.Empty<string>());

        Assert.Equal(2, state.StepCount);
        Assert.Equal(3, state.VisitedNodeIds.Count);
        Assert.Contains("start", state.VisitedNodeIds);
        Assert.Contains("mid", state.VisitedNodeIds);
        Assert.Contains("end", state.VisitedNodeIds);
    }

    [Fact]
    public void MissionRuntimeEngine_DoesNotAdvanceFromCompletedState()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-040",
            Title = "Completed",
            StartNodeId = "end",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "end",
                    Description = "End",
                    IsTerminal = true,
                    IsCheckpoint = false,
                    Transitions = Array.Empty<MissionTransition>()
                }
            }
        };

        var engine = new MissionRuntimeEngine();
        var state = engine.Start(definition);
        var next = engine.Advance(definition, state, Array.Empty<string>());

        Assert.Null(next);
        Assert.True(state.IsCompleted);
        Assert.Equal(0, state.StepCount);
    }
}
