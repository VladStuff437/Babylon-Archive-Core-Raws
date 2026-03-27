using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session046MissionRuntimeTests
{
    [Fact]
    public void MissionFactory_FallbackId_PreservesArchiveBasedMissionPrefix()
    {
        var address = new ArchiveAddress(4, 1, 0, 0, 0, 9);
        var missionId = $"mission-{address.ToCanonicalString()}";

        var definition = new MissionDefinition
        {
            MissionId = missionId,
            Title = "Archive Address",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode { NodeId = "start", Description = "Start", IsTerminal = false, IsCheckpoint = false, Transitions = System.Array.Empty<MissionTransition>() }
            }
        };

        var factory = new MissionFactory();
        var result = factory.Create(definition);

        Assert.True(result.UsedFallback);
        Assert.Equal($"{missionId}.fallback", result.Definition.MissionId);
    }
}
