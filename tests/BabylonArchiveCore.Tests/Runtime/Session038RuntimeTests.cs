using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Missions;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session038RuntimeTests
{
    [Fact]
    public void MissionNode_ValidateTransitions_RejectsDuplicateTargetsAndFallbacks()
    {
        var node = new MissionNode
        {
            NodeId = "n1",
            Description = "Node",
            IsTerminal = false,
            IsCheckpoint = false,
            Transitions = new[]
            {
                new MissionTransition { TargetNodeId = "a", Priority = 2, IsFallback = true },
                new MissionTransition { TargetNodeId = "a", Priority = 1, IsFallback = true }
            }
        };

        var errors = node.ValidateTransitions(new[] { "n1", "a" });
        Assert.Contains(errors, e => e.Contains("duplicate transition target", StringComparison.Ordinal));
        Assert.Contains(errors, e => e.Contains("more than one fallback", StringComparison.Ordinal));
    }

    [Fact]
    public void TransitionEvaluator_UsesFallback_WhenNoConditionsSatisfied()
    {
        var evaluator = new TransitionEvaluator();
        var selected = evaluator.SelectNextTransition(
            new[]
            {
                new MissionTransition { TargetNodeId = "locked", Priority = 10, ConditionKey = "cond.locked", IsFallback = false },
                new MissionTransition { TargetNodeId = "fallback", Priority = 1, ConditionKey = "cond.other", IsFallback = true }
            },
            new[] { "cond.none" });

        Assert.NotNull(selected);
        Assert.Equal("fallback", selected!.TargetNodeId);
    }

    [Fact]
    public void Migration038_AndSerializer_RoundTrip()
    {
        var migration = new Migration_038();
        var migrated = migration.Migrate(null);
        Assert.True(migrated.IsCheckpoint);

        var serializer = new Session038Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal("start", restored.NodeId);
    }
}
