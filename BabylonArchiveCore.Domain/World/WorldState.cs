namespace BabylonArchiveCore.Domain.World;

/// <summary>
/// Mutable state of the game world: consequence flags, moral/techno-arcane axes,
/// entity relations, and visited archive addresses.
/// </summary>
public sealed class WorldState
{
    public required int WorldSeed { get; init; }

    /// <summary>Key consequence events that shaped the world. Keyed by event identifier.</summary>
    public Dictionary<string, bool> ConsequenceFlags { get; init; } = new();

    /// <summary>Morality axis: -100 (ruthless) to +100 (compassionate).</summary>
    public int MoralAxis { get; set; }

    /// <summary>Techno-arcane alignment: -100 (pure tech) to +100 (pure arcane).</summary>
    public int TechnoArcaneAxis { get; set; }

    // Session 10 multi-axis morality state used by hub/mission bridges.
    public int CareAxis { get; set; }
    public int TruthAxis { get; set; }
    public int ResponsibilityAxis { get; set; }
    public int RespectAxis { get; set; }
    public int ArchiveIntegrityAxis { get; set; }
    public int InsightScore { get; set; }

    /// <summary>Reputation with named entities. Entity name → score (-100..+100).</summary>
    public Dictionary<string, int> EntityRelations { get; init; } = new();

    /// <summary>Canonical archive addresses already visited.</summary>
    public HashSet<string> VisitedAddresses { get; init; } = new();

    public void SetFlag(string flag, bool value = true) => ConsequenceFlags[flag] = value;

    public bool HasFlag(string flag) => ConsequenceFlags.TryGetValue(flag, out var v) && v;

    public void AdjustMoral(int delta) =>
        MoralAxis = Math.Clamp(MoralAxis + delta, -100, 100);

    public void ApplyMoralDelta(BabylonArchiveCore.Domain.World.Morality.MoralDelta delta)
    {
        CareAxis = Math.Clamp(CareAxis + delta.Care, -100, 100);
        TruthAxis = Math.Clamp(TruthAxis + delta.Truth, -100, 100);
        ResponsibilityAxis = Math.Clamp(ResponsibilityAxis + delta.Responsibility, -100, 100);
        RespectAxis = Math.Clamp(RespectAxis + delta.Respect, -100, 100);
        ArchiveIntegrityAxis = Math.Clamp(ArchiveIntegrityAxis + delta.ArchiveIntegrity, -100, 100);
        InsightScore = Math.Max(0, InsightScore + delta.Insight);

        // Keep legacy one-axis systems in sync with the richer vector.
        var composite = (CareAxis + TruthAxis + ResponsibilityAxis + RespectAxis + ArchiveIntegrityAxis) / 5;
        MoralAxis = composite;
    }

    public void AdjustTechnoArcane(int delta) =>
        TechnoArcaneAxis = Math.Clamp(TechnoArcaneAxis + delta, -100, 100);

    public void AdjustRelation(string entity, int delta)
    {
        EntityRelations.TryGetValue(entity, out var current);
        EntityRelations[entity] = Math.Clamp(current + delta, -100, 100);
    }
}
