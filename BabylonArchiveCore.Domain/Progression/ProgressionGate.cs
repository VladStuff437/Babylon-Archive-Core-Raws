namespace BabylonArchiveCore.Domain.Progression;

/// <summary>
/// A progression gate that blocks access to an archive zone until
/// level/stat/perk/schematic requirements are met.
/// </summary>
public sealed class ProgressionGate
{
    public required string Id { get; init; }

    /// <summary>Canonical archive address this gate protects.</summary>
    public required string TargetAddress { get; init; }

    /// <summary>Human-readable description.</summary>
    public string? Description { get; init; }

    /// <summary>All requirements must be met (AND logic).</summary>
    public List<ProgressionGateRequirement> Requirements { get; init; } = new();
}
