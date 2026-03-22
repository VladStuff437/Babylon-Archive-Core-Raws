namespace BabylonArchiveCore.Domain.Progression;

/// <summary>
/// Core operator stats that grow with progression and are checked by dialogue/perks/schematics.
/// </summary>
public enum StatType
{
    /// <summary>Analytical ability — affects Evidence checks and schematic research speed.</summary>
    Analysis,

    /// <summary>Social manipulation — affects Deception and Persuasion checks.</summary>
    Influence,

    /// <summary>Technical proficiency — affects tech-oriented schematics and terminal operations.</summary>
    Tech,

    /// <summary>Arcane affinity — affects arcane schematics and Hard-Archive navigation.</summary>
    Arcane,

    /// <summary>Endurance — affects timed missions and competition stamina.</summary>
    Endurance,
}
