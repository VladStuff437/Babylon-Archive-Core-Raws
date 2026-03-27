using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using BabylonArchiveCore.Runtime.Missions.Persistence;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session045MissionRuntimeTests
{
    [Fact]
    public void MissionRuntimePersistence_RestoreKeepsProgressionState()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-045",
            Title = "SaveLoad",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "start",
                    Description = "Start",
                    IsTerminal = false,
                    IsCheckpoint = false,
                    Transitions = new[] { new MissionTransition { TargetNodeId = "end", Priority = 1, IsFallback = true } }
                },
                new MissionNode { NodeId = "end", Description = "End", IsTerminal = true, IsCheckpoint = false, Transitions = System.Array.Empty<MissionTransition>() }
            }
        };

        var engine = new MissionRuntimeEngine();
        var state = engine.Start(definition);
        _ = engine.Advance(definition, state, System.Array.Empty<string>());

        var persistence = new MissionRuntimePersistence();
        var snapshot = persistence.SaveSnapshot(definition, state);
        var restored = persistence.RestoreState(snapshot);

        Assert.Equal(state.CurrentNodeId, restored.CurrentNodeId);
        Assert.Equal(state.IsCompleted, restored.IsCompleted);
        Assert.Equal(state.StepCount, restored.StepCount);



}    }
