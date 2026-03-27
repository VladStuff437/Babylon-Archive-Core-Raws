using BabylonArchiveCore.Core.Economy;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Economy;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session034CombatSmokeTests
{
    [Fact]
    public void EconomyTrade_Smoke()
    {
        var economy = new EconomyState();
        economy.AddCredits(1000);
        economy.SetPriceMultipliers(1.05f, 0.9f);

        var world = new WorldState();
        world.SetFactionReputation("guild", 40);

        var sync = new WorldEconomySynchronizer(economy, world);
        sync.ApplyFactionTransaction("guild", basePrice: 100, isPurchase: true);
        sync.ApplyFactionTransaction("guild", basePrice: 100, isPurchase: false);

        Assert.True(economy.Credits >= 0);
    }
}
