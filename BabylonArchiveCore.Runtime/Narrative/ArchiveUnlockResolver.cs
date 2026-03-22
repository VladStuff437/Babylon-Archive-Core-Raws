using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Narrative;
using BabylonArchiveCore.Domain.World;

namespace BabylonArchiveCore.Runtime.Narrative;

/// <summary>
/// Checks <see cref="ArchiveUnlockGate"/> requirements against
/// current <see cref="WorldState"/> and <see cref="InterventionTracker"/>,
/// returning which archive zones are now accessible.
/// </summary>
public sealed class ArchiveUnlockResolver
{
    private readonly ILogger _logger;

    public ArchiveUnlockResolver(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns the subset of gates whose requirements are ALL satisfied.
    /// </summary>
    public List<ArchiveUnlockGate> ResolveUnlocked(
        IReadOnlyList<ArchiveUnlockGate> gates,
        WorldState worldState,
        InterventionCertainty certainty)
    {
        var unlocked = new List<ArchiveUnlockGate>();

        foreach (var gate in gates)
        {
            if (IsGateSatisfied(gate, worldState, certainty))
            {
                unlocked.Add(gate);
                _logger.Info($"Unlock: gate '{gate.Id}' → address '{gate.TargetAddress}' is now accessible.");
            }
        }

        return unlocked;
    }

    /// <summary>
    /// Check a single gate: all requirements must be met (AND logic).
    /// </summary>
    public bool IsGateSatisfied(
        ArchiveUnlockGate gate,
        WorldState worldState,
        InterventionCertainty certainty)
    {
        foreach (var req in gate.Requirements)
        {
            if (!IsRequirementMet(req, worldState, certainty))
                return false;
        }
        return true;
    }

    private static bool IsRequirementMet(
        UnlockRequirement req,
        WorldState worldState,
        InterventionCertainty certainty)
    {
        return req.Type switch
        {
            UnlockRequirementType.Flag =>
                worldState.HasFlag(req.Key),

            UnlockRequirementType.RelationThreshold =>
                worldState.EntityRelations.TryGetValue(req.Key, out var rel) && rel >= req.Threshold,

            UnlockRequirementType.MoralThreshold =>
                req.Threshold >= 0
                    ? worldState.MoralAxis >= req.Threshold
                    : worldState.MoralAxis <= req.Threshold,

            UnlockRequirementType.AddressVisited =>
                worldState.VisitedAddresses.Contains(req.Key),

            UnlockRequirementType.CertaintyLevel =>
                (int)certainty >= req.Threshold,

            _ => false,
        };
    }
}
