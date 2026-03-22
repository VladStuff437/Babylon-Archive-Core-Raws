namespace BabylonArchiveCore.Domain.Progression;

/// <summary>
/// A progression-based gate requirement for archive zones.
/// </summary>
public sealed class ProgressionGateRequirement
{
    public required ProgressionGateType Type { get; init; }

    /// <summary>Key: stat name, perk id, schematic id, or capability name.</summary>
    public required string Key { get; init; }

    /// <summary>Threshold value (for level/stat checks).</summary>
    public int Threshold { get; init; }
}
