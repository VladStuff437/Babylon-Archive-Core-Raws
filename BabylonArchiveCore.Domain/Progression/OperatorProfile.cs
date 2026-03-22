namespace BabylonArchiveCore.Domain.Progression;

/// <summary>
/// The operator's progression profile: XP, level, stat scores, and unlocked perks.
/// </summary>
public sealed class OperatorProfile
{
    public int Experience { get; set; }
    public int Level { get; set; } = 1;

    /// <summary>Stat scores keyed by <see cref="StatType"/>.</summary>
    public Dictionary<StatType, int> Stats { get; init; } = new()
    {
        [StatType.Analysis] = 10,
        [StatType.Influence] = 10,
        [StatType.Tech] = 10,
        [StatType.Arcane] = 10,
        [StatType.Endurance] = 10,
    };

    /// <summary>Unlocked perk ids.</summary>
    public HashSet<string> UnlockedPerks { get; init; } = new();

    /// <summary>Stat points available for allocation after level-up.</summary>
    public int StatPointsAvailable { get; set; }

    public int GetStat(StatType stat) =>
        Stats.TryGetValue(stat, out var v) ? v : 0;

    public void AddStat(StatType stat, int delta)
    {
        Stats.TryGetValue(stat, out var current);
        Stats[stat] = Math.Max(0, current + delta);
    }
}
