using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Missions;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session044RuntimeTests
{
    [Fact]
    public void FallbackMissionProvider_IsDeterministic_ForEquivalentReasonSets()
    {
        var provider = new FallbackMissionProvider();

        var first = provider.CreateFallbackMission("mission-044", new[] { "B", "A", "B" });
        var second = provider.CreateFallbackMission("mission-044", new[] { "A", "B" });

        Assert.Equal(first.MissionId, second.MissionId);
        Assert.Equal(first.Title, second.Title);
    }

    [Fact]
    public void Migration044_AndSerializer_RoundTrip()
    {
        var migration = new Migration_044();
        var migrated = migration.Migrate(null);

        var serializer = new Session044Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal(44, restored.ContractVersion);
        Assert.True(restored.IsDeterministic);
    }
}
