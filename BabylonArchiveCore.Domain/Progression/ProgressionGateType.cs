namespace BabylonArchiveCore.Domain.Progression;

/// <summary>
/// Type of requirement for progression-based archive gate unlocks.
/// Extends the narrative unlock system with progression-specific checks.
/// </summary>
public enum ProgressionGateType
{
    /// <summary>Operator must be at least a certain level.</summary>
    MinLevel,

    /// <summary>A specific stat must reach a threshold.</summary>
    StatThreshold,

    /// <summary>A specific perk must be unlocked.</summary>
    PerkRequired,

    /// <summary>A specific schematic must be completed.</summary>
    SchematicCompleted,

    /// <summary>A specific capability must be available (from perks or schematics).</summary>
    CapabilityRequired,
}
