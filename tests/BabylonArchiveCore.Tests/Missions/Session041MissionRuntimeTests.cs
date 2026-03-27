using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session041MissionRuntimeTests
{
    [Fact]
    public void MissionFactory_UsesFallback_WhenReachabilityFails()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-041",
            Title = "Reachability",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode { NodeId = "start", Description = "Start", IsTerminal = false, Transitions = Array.Empty<MissionTransition>() },
                new MissionNode { NodeId = "orphan", Description = "Orphan", IsTerminal = true, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var factory = new MissionFactory();
        var result = factory.Create(definition);

        Assert.True(result.UsedFallback);
        Assert.EndsWith(".fallback", result.Definition.MissionId, StringComparison.Ordinal);
        Assert.NotEmpty(result.ValidationIssues);



}    }
