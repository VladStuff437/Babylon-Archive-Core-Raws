using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Missions;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session040RuntimeTests
{
    [Fact]
    public void MissionRuntimeEngine_StartAndAdvanceToTerminal()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-040",
            Title = "Mission Runtime",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "start",
                    Description = "Start",
                    IsTerminal = false,
                    IsCheckpoint = true,
                    Transitions = new[]
                    {
                        new MissionTransition { TargetNodeId = "end", Priority = 1, IsFallback = true }
                    }
                },
                new MissionNode
                {
                    NodeId = "end",
                    Description = "End",
                    IsTerminal = true,
                    IsCheckpoint = false,
                    Transitions = Array.Empty<MissionTransition>()
                }
            }
        };

        var engine = new MissionRuntimeEngine();
        var state = engine.Start(definition);
        var next = engine.Advance(definition, state, Array.Empty<string>());

        Assert.NotNull(next);
        Assert.Equal("end", next!.NodeId);
        Assert.True(state.IsCompleted);
        Assert.Equal(1, state.StepCount);
    }

    [Fact]
    public void MissionRuntimeEngine_Advance_UsesBalanceDrivenTransitionScoring()
    {
        var definition = new MissionDefinition
        {
            MissionId = "mission-040",
            Title = "Mission Runtime",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "start",
                    Description = "Start",
                    IsTerminal = false,
                    IsCheckpoint = true,
                    Transitions = new[]
                    {
                        new MissionTransition { TargetNodeId = "safe", Priority = 10, ConditionKey = "cond.ok", IsFallback = false },
                        new MissionTransition { TargetNodeId = "risky", Priority = 300, ConditionKey = "cond.ok", IsFallback = true }
                    }
                },
                new MissionNode { NodeId = "safe", Description = "Safe", IsTerminal = true, IsCheckpoint = false, Transitions = Array.Empty<MissionTransition>() },
                new MissionNode { NodeId = "risky", Description = "Risky", IsTerminal = true, IsCheckpoint = false, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"s040\",\"mission\":{\"priorityWeight\":1,\"satisfiedBonus\":0,\"fallbackPenalty\":400}}");

        var engine = new MissionRuntimeEngine();
        var state = engine.Start(definition);
        var next = engine.Advance(definition, state, new[] { "cond.ok" }, table);

        Assert.NotNull(next);
        Assert.Equal("safe", next!.NodeId);
    }

    [Fact]
    public void Migration040_AndSerializer_RoundTrip()
    {
        var migration = new Migration_040();
        var migrated = migration.Migrate(null);
        Assert.Equal(40, migrated.ContractVersion);

        var serializer = new Session040Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal("mission-040", restored.MissionId);
        Assert.Equal("start", restored.CurrentNodeId);
        Assert.Equal(128, restored.MaxStepCount);
    }
}
