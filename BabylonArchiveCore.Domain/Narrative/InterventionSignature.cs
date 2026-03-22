namespace BabylonArchiveCore.Domain.Narrative;

/// <summary>
/// A repeating pattern signature in the intervention trail.
/// When the same SignaturePattern appears across multiple addresses,
/// it indicates coordinated external manipulation.
/// </summary>
public sealed class InterventionSignature
{
    public required string PatternId { get; init; }

    /// <summary>Human-readable description of the repeating anomaly pattern.</summary>
    public required string Description { get; init; }

    /// <summary>Addresses where this pattern has been detected.</summary>
    public List<string> DetectedAtAddresses { get; init; } = new();

    /// <summary>How many detections are needed for this pattern to be considered confirmed.</summary>
    public int ConfirmationThreshold { get; init; } = 3;

    /// <summary>Whether the player has enough evidence to consider this pattern confirmed.</summary>
    public bool IsConfirmed => DetectedAtAddresses.Count >= ConfirmationThreshold;
}
