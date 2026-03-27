using BabylonArchiveCore.Core.Economy;
using BabylonArchiveCore.Core.State;

namespace BabylonArchiveCore.Runtime.Economy;

/// <summary>
/// Синхронизация экономического контура с WorldState-эффектами.
/// </summary>
public sealed class WorldEconomySynchronizer
{
    private readonly EconomyState economyState;
    private readonly WorldState worldState;

    public WorldEconomySynchronizer(EconomyState economyState, WorldState worldState)
    {
        ArgumentNullException.ThrowIfNull(economyState);
        ArgumentNullException.ThrowIfNull(worldState);

        this.economyState = economyState;
        this.worldState = worldState;
    }

    public void ApplyMissionEconomicEffect(int creditDelta, float buyMultiplier, float sellMultiplier, MissionEffect missionEffect)
    {
        economyState.AddCredits(creditDelta);
        economyState.SetPriceMultipliers(buyMultiplier, sellMultiplier);
        worldState.ApplyMissionEffect(missionEffect);
    }

    public void ApplyFactionTransaction(string factionId, int basePrice, bool isPurchase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(factionId);

        // Better reputation lowers buy prices and slightly improves sell prices.
        var reputation = worldState.GetFactionReputation(factionId);
        var factionModifier = 1f - Math.Clamp(reputation / 1000f, -0.2f, 0.2f);
        economyState.SetFactionModifier(factionId, factionModifier);

        var price = isPurchase
            ? economyState.QuoteBuyPrice(basePrice, factionId)
            : economyState.QuoteSellPrice(basePrice, factionId);

        if (isPurchase)
        {
            economyState.TrySpendCredits(price);
        }
        else
        {
            economyState.AddCredits(price);
        }
    }

    public void ApplyBalanceProfile(Balance.BalanceTable balanceTable)
    {
        ArgumentNullException.ThrowIfNull(balanceTable);

        var buy = balanceTable.GetScalar("economy.buyPriceMultiplier", economyState.BuyPriceMultiplier);
        var sell = balanceTable.GetScalar("economy.sellPriceMultiplier", economyState.SellPriceMultiplier);
        var inflation = balanceTable.GetScalar("economy.inflationIndex", economyState.InflationIndex);

        economyState.SetPriceMultipliers(buy, sell);
        economyState.SetInflationIndex(inflation);
    }
}
