using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session042MissionRuntimeTests
{
    [Fact]
    public void MissionFactory_UsesFallback_WhenDeadEndDetected()
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

        var factory = new MissionFactory();
        var result = factory.Create(definition);

        Assert.True(result.UsedFallback);
        Assert.Contains(result.ValidationIssues, issue => issue.Code == "MVAL-042-DEADEND");
    }
}
