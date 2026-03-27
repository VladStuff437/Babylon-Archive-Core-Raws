using BabylonArchiveCore.Core.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Missions;

public class Session037MissionRuntimeTests
{
    [Fact]
    public void MissionDefinition_Validate_ReportsMissingStartAndUnknownTarget()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-037",
            Title = "Mission Definition",
            StartNodeId = "missing",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "n1",
                    Description = "Node 1",
                    IsTerminal = false,
                    IsCheckpoint = false,
                    Transitions = new[]
                    {
                        new MissionTransition { TargetNodeId = "ghost", Priority = 1, IsFallback = false }
                    }
                }
            }
        };

        var errors = definition.Validate();
        Assert.Contains(errors, e => e.Contains("Start node", StringComparison.Ordinal));
        Assert.Contains(errors, e => e.Contains("unknown target", StringComparison.Ordinal));
    }
}
