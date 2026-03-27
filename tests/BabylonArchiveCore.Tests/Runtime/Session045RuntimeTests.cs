using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Missions;
using BabylonArchiveCore.Runtime.Missions.Persistence;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session045RuntimeTests
{
    [Fact]
    public void MissionRuntimePersistence_SnapshotContainsChecksum_AndRestores()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-045",
            Title = "Persist",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode { NodeId = "start", Description = "Start", IsTerminal = true, IsCheckpoint = false, Transitions = System.Array.Empty<MissionTransition>() }
            }
        };

        var runtime = new MissionRuntimeEngine();
        var state = runtime.Start(definition);

        var persistence = new MissionRuntimePersistence();
        var snapshot = persistence.SaveSnapshot(definition, state);

        Assert.Equal(45, snapshot.SnapshotVersion);
        Assert.False(string.IsNullOrWhiteSpace(snapshot.StateChecksum));

        var json = persistence.Serialize(snapshot);
        var restoredSnapshot = persistence.Deserialize(json);
        var restoredState = persistence.RestoreState(restoredSnapshot);

        Assert.Equal(state.CurrentNodeId, restoredState.CurrentNodeId);
        Assert.Equal(state.IsCompleted, restoredState.IsCompleted);
    }

    [Fact]
    public void MissionRuntimePersistence_ThrowsOnChecksumMismatch()
    {
        var persistence = new MissionRuntimePersistence();
        var tampered = new MissionRuntimeSnapshot
        {
            MissionId = "mission-045",
            SnapshotVersion = 45,
            CurrentNodeId = "start",
            IsCompleted = false,
            StepCount = 0,
            VisitedNodeIds = new[] { "start" },
            StateChecksum = "BAD"
        };

        var json = persistence.Serialize(tampered);
        Assert.Throws<System.InvalidOperationException>(() => persistence.Deserialize(json));
    }

    [Fact]
    public void Migration045_AndSerializer_RoundTrip()
    {
        var migration = new Migration_045();
        var migrated = migration.Migrate(null);

        var serializer = new Session045Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal(45, restored.ContractVersion);
        Assert.Equal(45, restored.SnapshotVersion);
    }
}
