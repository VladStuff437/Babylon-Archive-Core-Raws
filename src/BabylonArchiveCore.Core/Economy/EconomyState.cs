namespace BabylonArchiveCore.Core.Economy;

/// <summary>
/// Базовый экономический контур.
/// </summary>
public sealed class EconomyState
{
    private readonly Dictionary<string, float> factionPriceModifiers = new(StringComparer.Ordinal);

    public int Credits { get; private set; }

    public float BuyPriceMultiplier { get; private set; } = 1f;

    public float SellPriceMultiplier { get; private set; } = 1f;

    public float InflationIndex { get; private set; } = 1f;

    public void AddCredits(int amount)
    {
        Credits = Math.Max(0, Credits + amount);
    }

    public bool TrySpendCredits(int amount)
    {
        if (amount < 0 || amount > Credits)
        {
            return false;
        }

        Credits -= amount;
        return true;
    }

    public void SetPriceMultipliers(float buyMultiplier, float sellMultiplier)
    {
        BuyPriceMultiplier = Math.Max(0f, buyMultiplier);
        SellPriceMultiplier = Math.Max(0f, sellMultiplier);
    }

    public void SetInflationIndex(float value)
    {
        InflationIndex = Math.Clamp(value, 0.1f, 5f);
    }

    public void SetFactionModifier(string factionId, float modifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(factionId);
        factionPriceModifiers[factionId] = Math.Clamp(modifier, 0.5f, 1.5f);
    }

    public float GetFactionModifier(string factionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(factionId);
        return factionPriceModifiers.TryGetValue(factionId, out var modifier) ? modifier : 1f;
    }

    public int QuoteBuyPrice(int basePrice, string? factionId = null)
    {
        var faction = string.IsNullOrWhiteSpace(factionId) ? 1f : GetFactionModifier(factionId);
        var computed = Math.Max(0f, basePrice) * BuyPriceMultiplier * InflationIndex * faction;
        return (int)Math.Round(computed, MidpointRounding.AwayFromZero);
    }

    public int QuoteSellPrice(int basePrice, string? factionId = null)
    {
        var faction = string.IsNullOrWhiteSpace(factionId) ? 1f : GetFactionModifier(factionId);
        var computed = Math.Max(0f, basePrice) * SellPriceMultiplier * InflationIndex / faction;
        return (int)Math.Round(Math.Max(0f, computed), MidpointRounding.AwayFromZero);
    }
}
