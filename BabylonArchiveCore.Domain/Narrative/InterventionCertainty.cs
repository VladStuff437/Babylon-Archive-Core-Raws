namespace BabylonArchiveCore.Domain.Narrative;

/// <summary>
/// Certainty level of the player's accumulated intervention evidence.
/// Progress through these levels drives the narrative spine.
/// </summary>
public enum InterventionCertainty
{
    /// <summary>No evidence collected yet.</summary>
    Unaware = 0,

    /// <summary>Minor inconsistencies noticed but no pattern.</summary>
    Suspicious = 1,

    /// <summary>Clear pattern forming; multiple discrepancies connected.</summary>
    Investigating = 2,

    /// <summary>Strong evidence of coordinated manipulation.</summary>
    Convinced = 3,

    /// <summary>Undeniable proof; narrative climax triggers.</summary>
    Proven = 4,
}
