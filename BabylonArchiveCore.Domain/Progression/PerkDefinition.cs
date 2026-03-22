namespace BabylonArchiveCore.Domain.Progression;

/// <summary>
/// A perk that provides a capability unlock (not just a numeric bonus).
/// </summary>
public sealed class PerkDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    /// <summary>Minimum operator level required.</summary>
    public int RequiredLevel { get; init; } = 1;

    /// <summary>Stat requirements: stat type → minimum value.</summary>
    public Dictionary<StatType, int> RequiredStats { get; init; } = new();

    /// <summary>Perk ids that must be unlocked first (prerequisites).</summary>
    public List<string> PrerequisitePerks { get; init; } = new();

    /// <summary>Capability unlocked by this perk (e.g., "deep_scan", "arcane_sight").</summary>
    public required string GrantsCapability { get; init; }
}
