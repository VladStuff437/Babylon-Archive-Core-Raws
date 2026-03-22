namespace BabylonArchiveCore.Domain.Mission;

/// <summary>
/// A consequence effect applied to WorldState when a mission completes.
/// </summary>
public sealed class MissionEffect
{
    /// <summary>Consequence flag to set (null = skip).</summary>
    public string? SetFlag { get; init; }

    /// <summary>Moral axis delta (0 = no change).</summary>
    public int MoralDelta { get; init; }

    /// <summary>Techno-arcane axis delta (0 = no change).</summary>
    public int TechnoArcaneDelta { get; init; }

    /// <summary>Entity relation adjustments. Entity name → delta.</summary>
    public Dictionary<string, int> RelationDeltas { get; init; } = new();
}
