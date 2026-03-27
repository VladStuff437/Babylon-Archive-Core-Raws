using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Missions;
using BabylonArchiveCore.Runtime.Missions.Persistence;
using BabylonArchiveCore.Runtime.Missions.Validation;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session042RuntimeTests
{
    [Fact]
    public void DeadEndValidator_FindsNonTerminalDeadEnd()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-042",
            Title = "DeadEnd",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode { NodeId = "start", Description = "Start", IsTerminal = false, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var validator = new DeadEndValidator();
        var result = validator.Validate(definition);

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "MVAL-042-DEADEND" && issue.NodeId == "start");
    }

    [Fact]
    public void MissionRuntimePersistence_SaveLoadRoundTrip()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-042",
            Title = "Persist",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode { NodeId = "start", Description = "Start", IsTerminal = true, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var runtime = new MissionRuntimeEngine();
        var state = runtime.Start(definition);

        var persistence = new MissionRuntimePersistence();
        var snapshot = persistence.SaveSnapshot(definition, state);
        var restored = persistence.RestoreState(snapshot);

        Assert.Equal(state.CurrentNodeId, restored.CurrentNodeId);
        Assert.Equal(state.IsCompleted, restored.IsCompleted);
    }

    [Fact]
    public void Migration042_AndSerializer_RoundTrip()
    {
        var migration = new Migration_042();
        var migrated = migration.Migrate(null);

        var serializer = new Session042Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal(42, restored.ContractVersion);
        Assert.True(restored.AllowsFallback);
    }
}
