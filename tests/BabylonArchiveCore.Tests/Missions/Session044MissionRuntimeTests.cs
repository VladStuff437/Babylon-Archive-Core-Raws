using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session044MissionRuntimeTests
{
    [Fact]
    public void MissionFactory_UsesDeterministicFallback_ForInvalidMission()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-044",
            Title = "Fallback",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode { NodeId = "start", Description = "Start", IsTerminal = false, IsCheckpoint = false, Transitions = System.Array.Empty<MissionTransition>() }
            }
        };

        var factory = new MissionFactory();
        var result = factory.Create(definition);

        Assert.True(result.UsedFallback);
        Assert.Equal("mission-044.fallback", result.Definition.MissionId);
        Assert.Contains("MVAL-042-DEADEND", result.ValidationIssues.Select(i => i.Code));
    }
}
