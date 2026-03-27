namespace BabylonArchiveCore.Core.State;

/// <summary>
/// Состояние мира с репутациями фракций и осями выбора.
/// </summary>
public sealed class WorldState
{
    public Dictionary<string, int> FactionReputation { get; } = new(StringComparer.Ordinal);

    public int WorldAxisVersion { get; private set; } = 35;

    public float MoralAxis { get; private set; }

    public float TechnoArcaneAxis { get; private set; }

    public void SetAxes(float moralAxis, float technoArcaneAxis)
    {
        MoralAxis = Math.Clamp(moralAxis, -100f, 100f);
        TechnoArcaneAxis = Math.Clamp(technoArcaneAxis, -100f, 100f);
        WorldAxisVersion = Math.Max(WorldAxisVersion, 36);
    }

    public void ApplyAxisDelta(float moralDelta, float technoArcaneDelta)
    {
        MoralAxis = Math.Clamp(MoralAxis + moralDelta, -100f, 100f);
        TechnoArcaneAxis = Math.Clamp(TechnoArcaneAxis + technoArcaneDelta, -100f, 100f);
        WorldAxisVersion = Math.Max(WorldAxisVersion, 36);
    }

    public (float MoralAxis, float TechnoArcaneAxis) GetAxisSnapshot()
    {
        return (MoralAxis, TechnoArcaneAxis);
    }

    public void SetFactionReputation(string factionId, int value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(factionId);
        FactionReputation[factionId] = Math.Clamp(value, -100, 100);
    }

    public int GetFactionReputation(string factionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(factionId);
        return FactionReputation.TryGetValue(factionId, out var value) ? value : 0;
    }

    public int ChangeFactionReputation(string factionId, int delta)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(factionId);
        var updated = GetFactionReputation(factionId) + delta;
        SetFactionReputation(factionId, updated);
        return GetFactionReputation(factionId);
    }

    public void ApplyMissionEffect(MissionEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);

        ApplyAxisDelta(effect.MoralDelta, effect.TechnoArcaneDelta);

        foreach (var reputationChange in effect.FactionReputationDelta)
        {
            var current = GetFactionReputation(reputationChange.Key);
            SetFactionReputation(reputationChange.Key, current + reputationChange.Value);
        }
    }
}

public sealed class MissionEffect
{
    public float MoralDelta { get; init; }

    public float TechnoArcaneDelta { get; init; }

    public IReadOnlyDictionary<string, int> FactionReputationDelta { get; init; } = new Dictionary<string, int>(StringComparer.Ordinal);
}
