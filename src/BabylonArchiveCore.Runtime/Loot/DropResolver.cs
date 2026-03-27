namespace BabylonArchiveCore.Runtime.Loot;

/// <summary>
/// Resolver выпадения лута по весам редкости.
/// </summary>
public sealed class DropResolver
{
    public string ResolveDropTier(IReadOnlyDictionary<string, int> rarityWeights, int roll, float luckBias = 0f)
    {
        ArgumentNullException.ThrowIfNull(rarityWeights);

        if (rarityWeights.Count == 0)
        {
            return "common";
        }

        var adjusted = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var entry in rarityWeights)
        {
            var weight = Math.Max(0, entry.Value);
            if (weight == 0)
            {
                continue;
            }

            // Luck slightly boosts non-common outcomes while preserving deterministic roll path.
            var boost = entry.Key.Equals("common", StringComparison.OrdinalIgnoreCase)
                ? 1f
                : 1f + Math.Clamp(luckBias, 0f, 1f);
            adjusted[entry.Key] = Math.Max(1, (int)Math.Round(weight * boost, MidpointRounding.AwayFromZero));
        }

        return ResolveRarity(adjusted, roll);
    }

    public string ResolveRarity(IReadOnlyDictionary<string, int> rarityWeights, int roll)
    {
        ArgumentNullException.ThrowIfNull(rarityWeights);

        if (rarityWeights.Count == 0)
        {
            return "common";
        }

        var total = rarityWeights.Values.Where(v => v > 0).Sum();
        if (total <= 0)
        {
            return "common";
        }

        var normalizedRoll = Math.Clamp(roll, 0, total - 1);
        var cursor = 0;
        foreach (var kvp in rarityWeights)
        {
            var weight = Math.Max(0, kvp.Value);
            cursor += weight;
            if (normalizedRoll < cursor)
            {
                return kvp.Key;
            }
        }

        return rarityWeights.Keys.First();
    }

    public DropEntry ResolveDrop(IReadOnlyDictionary<string, int> rarityWeights, IReadOnlyList<string> candidates, int roll, float luckBias = 0f)
    {
        ArgumentNullException.ThrowIfNull(rarityWeights);
        ArgumentNullException.ThrowIfNull(candidates);

        var tier = ResolveDropTier(rarityWeights, roll, luckBias);
        if (candidates.Count == 0)
        {
            return new DropEntry
            {
                ItemId = "none",
                Rarity = tier,
                Quantity = 0
            };
        }

        var normalized = Math.Clamp(roll, 0, int.MaxValue - 1);
        var index = normalized % candidates.Count;

        return new DropEntry
        {
            ItemId = candidates[index],
            Rarity = tier,
            Quantity = tier.Equals("legendary", StringComparison.OrdinalIgnoreCase) ? 1 : 1 + (normalized % 2)
        };
    }

    public DropEntry ResolveDropFromBalance(Balance.BalanceTable balanceTable, IReadOnlyList<string> candidates, int roll)
    {
        ArgumentNullException.ThrowIfNull(balanceTable);
        ArgumentNullException.ThrowIfNull(candidates);

        var rarityWeights = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            ["common"] = balanceTable.GetWeight("common", 60),
            ["uncommon"] = balanceTable.GetWeight("uncommon", 25),
            ["rare"] = balanceTable.GetWeight("rare", 10),
            ["epic"] = balanceTable.GetWeight("epic", 4),
            ["legendary"] = balanceTable.GetWeight("legendary", 1)
        };

        var luckBias = balanceTable.GetScalar("loot.luckBias", 0f);
        return ResolveDrop(rarityWeights, candidates, roll, luckBias);
    }
}

public sealed class DropEntry
{
    public required string ItemId { get; init; }

    public required string Rarity { get; init; }

    public int Quantity { get; init; }
}
