using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Missions;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session039RuntimeTests
{
    [Fact]
    public void TransitionEvaluator_SelectNextTransitionWithBalance_PrefersNonFallbackWhenPenaltyHigh()
    {
        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"s039\",\"mission\":{\"priorityWeight\":1,\"satisfiedBonus\":0,\"fallbackPenalty\":300}}");

        var evaluator = new TransitionEvaluator();
        var selected = evaluator.SelectNextTransitionWithBalance(
            new[]
            {
                new MissionTransition { TargetNodeId = "safe", Priority = 50, IsFallback = false },
                new MissionTransition { TargetNodeId = "fallback", Priority = 200, IsFallback = true }
            },
            Array.Empty<string>(),
            table);

        Assert.NotNull(selected);
        Assert.Equal("safe", selected!.TargetNodeId);
    }

    [Fact]
    public void TransitionEvaluator_SelectNextTransitionWithBalance_UsesFallbackWhenNoSatisfiedTransitions()
    {
        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"s039\",\"mission\":{\"priorityWeight\":1,\"satisfiedBonus\":1000,\"fallbackPenalty\":250}}");

        var evaluator = new TransitionEvaluator();
        var selected = evaluator.SelectNextTransitionWithBalance(
            new[]
            {
                new MissionTransition { TargetNodeId = "locked", Priority = 10, ConditionKey = "cond.locked", IsFallback = false },
                new MissionTransition { TargetNodeId = "recover", Priority = 1, ConditionKey = "cond.recover", IsFallback = true }
            },
            Array.Empty<string>(),
            table);

        Assert.NotNull(selected);
        Assert.Equal("recover", selected!.TargetNodeId);
    }

    [Fact]
    public void Migration039_AndSerializer_RoundTrip()
    {
        var migration = new Migration_039();
        var migrated = migration.Migrate(null);
        Assert.Equal(39, migrated.ContractVersion);

        var serializer = new Session039Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal(39, restored.ContractVersion);
        Assert.True(restored.DeterministicTieBreak);
    }
}
