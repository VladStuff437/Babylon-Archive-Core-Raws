using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Missions.Validation;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session041RuntimeTests
{
    [Fact]
    public void ReachabilityValidator_FindsUnreachableNodes()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-041",
            Title = "Reachability",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode { NodeId = "start", Description = "Start", IsTerminal = false, Transitions = new[] { new MissionTransition { TargetNodeId = "end", Priority = 1 } } },
                new MissionNode { NodeId = "end", Description = "End", IsTerminal = true, Transitions = Array.Empty<MissionTransition>() },
                new MissionNode { NodeId = "orphan", Description = "Orphan", IsTerminal = true, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var validator = new ReachabilityValidator();
        var result = validator.Validate(definition);

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "MVAL-041-UNREACHABLE" && issue.NodeId == "orphan");
    }

    [Fact]
    public void ArchiveAddress_UsesCanonicalRoundTrip()
    {
        var address = new ArchiveAddress(2, 4, 1, 3, 5, 8);
        var canonical = address.ToCanonicalString();

        var parsed = ArchiveAddress.Parse(canonical);
        Assert.Equal(address, parsed);
    }

    [Fact]
    public void Migration041_AndSerializer_RoundTrip()
    {
        var migration = new Migration_041();
        var migrated = migration.Migrate(null);

        var serializer = new Session041Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal(41, restored.ContractVersion);
        Assert.Equal("mission-041", restored.MissionId);
    }
}
