namespace BabylonArchiveCore.Domain.Narrative;

/// <summary>
/// A single unlock precondition: what must be true in WorldState
/// for an archive zone to become accessible.
/// </summary>
public sealed class UnlockRequirement
{
    public required UnlockRequirementType Type { get; init; }

    /// <summary>Key: flag name, entity name, or address — depending on Type.</summary>
    public required string Key { get; init; }

    /// <summary>Threshold value for relation, moral, or certainty checks.</summary>
    public int Threshold { get; init; }
}
