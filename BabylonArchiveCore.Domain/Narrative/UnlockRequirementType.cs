namespace BabylonArchiveCore.Domain.Narrative;

/// <summary>
/// Type of requirement for unlocking archive zones.
/// </summary>
public enum UnlockRequirementType
{
    /// <summary>A specific consequence flag must be set.</summary>
    Flag,

    /// <summary>Relation with an entity must meet a threshold.</summary>
    RelationThreshold,

    /// <summary>Moral axis must be ≥ threshold (or ≤ if negative).</summary>
    MoralThreshold,

    /// <summary>A specific archive address must have been visited.</summary>
    AddressVisited,

    /// <summary>Intervention certainty must be ≥ specified level.</summary>
    CertaintyLevel,
}
