namespace BabylonArchiveCore.Runtime.Progression;

/// <summary>
/// Defines the balance curve: XP thresholds, stat points per level,
/// and tome difficulty scaling for the first ~5 tomes.
/// </summary>
public sealed class BalanceCurve
{
    /// <summary>Base XP needed for level 2.</summary>
    public int BaseXp { get; init; } = 100;

    /// <summary>Scaling factor per level (polynomial).</summary>
    public double GrowthExponent { get; init; } = 1.5;

    /// <summary>Stat points awarded per level-up.</summary>
    public int StatPointsPerLevel { get; init; } = 3;

    /// <summary>
    /// Total cumulative XP required to reach a given level.
    /// Level 1 requires 0, level 2 requires BaseXp, etc.
    /// </summary>
    public int XpForLevel(int level)
    {
        if (level <= 1) return 0;
        return (int)(BaseXp * Math.Pow(level - 1, GrowthExponent));
    }

    /// <summary>
    /// Returns the recommended operator level for a tome (1-based).
    /// Tome 1 → level 1, Tome 2 → level 3, etc.
    /// </summary>
    public int RecommendedLevelForTome(int tome) => tome switch
    {
        <= 1 => 1,
        2 => 3,
        3 => 6,
        4 => 10,
        5 => 15,
        _ => 15 + (tome - 5) * 5,
    };

    /// <summary>
    /// Returns the XP needed to reach the recommended level for a given tome.
    /// </summary>
    public int XpForTome(int tome) => XpForLevel(RecommendedLevelForTome(tome));
}
