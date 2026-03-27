using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Missions.Validation;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session043RuntimeTests
{
    [Fact]
    public void CycleSafetyValidator_FindsUnsafeCycleWithoutExit()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-043",
            Title = "Cycle",
            StartNodeId = "a",
            Nodes = new[]
            {
                new MissionNode { NodeId = "a", Description = "A", IsTerminal = false, Transitions = new[] { new MissionTransition { TargetNodeId = "b", Priority = 1 } } },
                new MissionNode { NodeId = "b", Description = "B", IsTerminal = false, Transitions = new[] { new MissionTransition { TargetNodeId = "a", Priority = 1 } } }
            }
        };

        var validator = new CycleSafetyValidator();
        var result = validator.Validate(definition);

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "MVAL-043-UNSAFE-CYCLE");
    }

    [Fact]
    public void ArchiveSeed_IsDeterministic()
    {
        var address = new ArchiveAddress(1, 2, 3, 4, 5, 6);
        var first = ArchiveSeed.ToSeed(address, worldSeed: 99);
        var second = ArchiveSeed.ToSeed(address, worldSeed: 99);

        Assert.Equal(first, second);
        Assert.True(first > 0);
    }

    [Fact]
    public void Migration043_AndSerializer_RoundTrip()
    {
        var migration = new Migration_043();
        var migrated = migration.Migrate(null);

        var serializer = new Session043Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal(43, restored.ContractVersion);
        Assert.Equal(8, restored.MaxAllowedCycleLength);
    }
}
