using BabylonArchiveCore.Core.Economy;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using BabylonArchiveCore.Runtime.Economy;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session034RuntimeTests
{
    [Fact]
    public void EconomyState_QuoteBuySell_UsesInflationAndFactionModifiers()
    {
        var economy = new EconomyState();
        economy.SetPriceMultipliers(1.1f, 0.8f);
        economy.SetInflationIndex(1.2f);
        economy.SetFactionModifier("guild", 0.9f);

        var buy = economy.QuoteBuyPrice(100, "guild");
        var sell = economy.QuoteSellPrice(100, "guild");

        Assert.Equal(119, buy);
        Assert.Equal(107, sell);
    }

    [Fact]
    public void WorldEconomySynchronizer_AppliesFactionTransactions()
    {
        var economy = new EconomyState();
        economy.AddCredits(500);

        var world = new WorldState();
        world.SetFactionReputation("guild", 50);

        var synchronizer = new WorldEconomySynchronizer(economy, world);
        synchronizer.ApplyFactionTransaction("guild", basePrice: 100, isPurchase: true);

        Assert.True(economy.Credits < 500);
    }

    [Fact]
    public void Migration034_AndSerializer_RoundTrip()
    {
        var migration = new Migration_034();
        var migrated = migration.Migrate(null);

        var serializer = new Session034Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal("session-034", restored.EconomyProfileId);
        Assert.Equal(1f, restored.InflationIndex);
    }
}
