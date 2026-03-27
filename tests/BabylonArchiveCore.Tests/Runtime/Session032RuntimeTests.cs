using BabylonArchiveCore.Core.Economy;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Economy;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using BabylonArchiveCore.Runtime.State;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session032RuntimeTests
{
    [Fact]
    public void WorldState_AppliesFactionAndAxisChanges()
    {
        var worldState = new WorldState();
        worldState.SetFactionReputation("guild", 10);

        worldState.ApplyMissionEffect(new MissionEffect
        {
            MoralDelta = 5f,
            TechnoArcaneDelta = -2f,
            FactionReputationDelta = new Dictionary<string, int>
            {
                ["guild"] = 15,
                ["order"] = -8
            }
        });

        Assert.Equal(25, worldState.GetFactionReputation("guild"));
        Assert.Equal(-8, worldState.GetFactionReputation("order"));
        Assert.Equal(5f, worldState.MoralAxis);
        Assert.Equal(-2f, worldState.TechnoArcaneAxis);
    }

    [Fact]
    public void WorldStateService_DelegatesWorldMutations()
    {
        var worldState = new WorldState();
        var service = new WorldStateService(worldState);

        service.SetFactionReputation("archive", 42);
        Assert.Equal(42, service.GetFactionReputation("archive"));
    }

    [Fact]
    public void BalanceTableLoader_LoadsSession032Shape()
    {
        var loader = new BalanceTableLoader();
        var json = "{\"profileId\":\"s032\",\"damageMultipliers\":{\"player\":1.2},\"economy\":{\"buyPriceMultiplier\":0.9}}";

        var table = loader.LoadFromJson(json);

        Assert.Equal("s032", table.ProfileId);
        Assert.Equal(1.2f, table.Scalars["damageMultipliers.player"]);
        Assert.Equal(0.9f, table.Scalars["economy.buyPriceMultiplier"]);
    }

    [Fact]
    public void Migration032_AndSerializer_RoundTrip()
    {
        var migration = new Migration_032();
        var migrated = migration.Migrate(null);
        Assert.Equal("baseline-v1", migrated.BalanceProfileId);

        var serializer = new Session032Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal("baseline-v1", restored.BalanceProfileId);
    }

    [Fact]
    public void SessionBalanceCatalog_RegisterAndResolve()
    {
        var catalog = new SessionBalanceCatalog();
        var table = new BalanceTable
        {
            ProfileId = "baseline",
            Scalars = new Dictionary<string, float>
            {
                ["damageMultipliers.player"] = 1.1f
            }
        };

        catalog.Register("032", table);
        var found = catalog.TryGet("032", out var restored);

        Assert.True(found);
        Assert.NotNull(restored);
        Assert.Equal(1.1f, restored!.Scalars["damageMultipliers.player"]);
    }

    [Fact]
    public void WorldEconomySynchronizer_AppliesEconomyAndMissionEffects()
    {
        var economy = new EconomyState();
        var worldState = new WorldState();
        var synchronizer = new WorldEconomySynchronizer(economy, worldState);

        synchronizer.ApplyMissionEconomicEffect(
            creditDelta: 100,
            buyMultiplier: 0.95f,
            sellMultiplier: 0.7f,
            missionEffect: new MissionEffect
            {
                MoralDelta = 2f,
                TechnoArcaneDelta = 1f,
                FactionReputationDelta = new Dictionary<string, int>
                {
                    ["archive"] = 5
                }
            });

        Assert.Equal(100, economy.Credits);
        Assert.Equal(0.95f, economy.BuyPriceMultiplier);
        Assert.Equal(0.7f, economy.SellPriceMultiplier);
        Assert.Equal(5, worldState.GetFactionReputation("archive"));
        Assert.Equal(2f, worldState.MoralAxis);
        Assert.Equal(1f, worldState.TechnoArcaneAxis);
    }
}
