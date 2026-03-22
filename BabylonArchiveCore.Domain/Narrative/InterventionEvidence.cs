namespace BabylonArchiveCore.Domain.Narrative;

/// <summary>
/// A discrepancy between terminal data and Hard-Archive data
/// discovered at a specific archive address — evidence of external intervention.
/// </summary>
public sealed class InterventionEvidence
{
    public required string Id { get; init; }

    /// <summary>Canonical archive address where the discrepancy was found.</summary>
    public required string Address { get; init; }

    /// <summary>What the terminal reports for this address.</summary>
    public required string TerminalData { get; init; }

    /// <summary>What the Hard-Archive actually contains.</summary>
    public required string ArchiveData { get; init; }

    /// <summary>Severity: 1 (minor inconsistency) to 5 (undeniable proof).</summary>
    public int Severity { get; init; } = 1;

    /// <summary>Human-readable description of the anomaly.</summary>
    public string? Description { get; init; }
}
