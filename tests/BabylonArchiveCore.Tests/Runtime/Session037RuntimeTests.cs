using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session037RuntimeTests
{
    [Fact]
    public void MissionDefinition_Validate_RejectsTerminalStartNode()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-037",
            Title = "Definition Contract",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "start",
                    Description = "Start",
                    IsTerminal = true,
                    IsCheckpoint = false,
                    Transitions = Array.Empty<MissionTransition>()
                }
            }
        };

        var errors = definition.Validate();
        Assert.Contains(errors, e => e.Contains("cannot be terminal", StringComparison.Ordinal));
    }

    [Fact]
    public void Migration037_AndSerializer_RoundTrip()
    {
        var migration = new Migration_037();
        var migrated = migration.Migrate(null);
        Assert.Equal(37, migrated.ContractVersion);

        var serializer = new Session037Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal("mission-037", restored.MissionId);
        Assert.Equal(37, restored.ContractVersion);
    }

    [Fact]
    public void MissionDefinition_Validate_CollectsNodeTransitionErrors()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-037",
            Title = "Definition Contract",
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
                        new MissionTransition { TargetNodeId = "missing", Priority = 1, IsFallback = false }
                    }
                }
            }
        };

        var errors = definition.Validate();
        Assert.Contains(errors, e => e.Contains("unknown target", StringComparison.Ordinal));
    }
}
