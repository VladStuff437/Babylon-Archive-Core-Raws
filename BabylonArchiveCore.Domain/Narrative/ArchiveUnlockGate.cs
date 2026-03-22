namespace BabylonArchiveCore.Domain.Narrative;

/// <summary>
/// A gate that blocks access to an archive zone (room/module/tome)
/// until all requirements are satisfied.
/// </summary>
public sealed class ArchiveUnlockGate
{
    public required string Id { get; init; }

    /// <summary>Canonical archive address this gate protects.</summary>
    public required string TargetAddress { get; init; }

    /// <summary>Human-readable description of what this gate guards.</summary>
    public string? Description { get; init; }

    /// <summary>All requirements must be met (AND logic).</summary>
    public List<UnlockRequirement> Requirements { get; init; } = new();
}
