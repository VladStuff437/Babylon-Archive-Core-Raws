using BabylonArchiveCore.Domain.Progression;

namespace BabylonArchiveCore.Runtime.Progression;

/// <summary>
/// Evaluates progression gates against an OperatorProfile and available capabilities.
/// </summary>
public static class ProgressionGateResolver
{
    /// <summary>
    /// Returns true if the operator meets all gate requirements.
    /// </summary>
    public static bool CanPass(
        ProgressionGate gate,
        OperatorProfile profile,
        HashSet<string> capabilities)
    {
        foreach (var req in gate.Requirements)
        {
            if (!MeetsRequirement(req, profile, capabilities))
                return false;
        }
        return true;
    }

    private static bool MeetsRequirement(
        ProgressionGateRequirement req,
        OperatorProfile profile,
        HashSet<string> capabilities) => req.Type switch
    {
        ProgressionGateType.MinLevel => profile.Level >= req.Threshold,
        ProgressionGateType.StatThreshold =>
            Enum.TryParse<StatType>(req.Key, out var stat) && profile.GetStat(stat) >= req.Threshold,
        ProgressionGateType.PerkRequired => profile.UnlockedPerks.Contains(req.Key),
        ProgressionGateType.SchematicCompleted => capabilities.Contains(req.Key) || profile.UnlockedPerks.Contains(req.Key),
        ProgressionGateType.CapabilityRequired => capabilities.Contains(req.Key),
        _ => false,
    };
}
