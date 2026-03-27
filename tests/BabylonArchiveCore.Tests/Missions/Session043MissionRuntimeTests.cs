using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session043MissionRuntimeTests
{
    [Fact]
    public void MissionFactory_UsesFallback_WhenUnsafeCycleDetected()
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

        var factory = new MissionFactory();
        var result = factory.Create(definition);

        Assert.True(result.UsedFallback);
        Assert.Contains(result.ValidationIssues, issue => issue.Code == "MVAL-043-UNSAFE-CYCLE");
    }
}
