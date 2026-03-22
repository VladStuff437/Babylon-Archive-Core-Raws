using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Core.Scene;

/// <summary>
/// Result of firing triggers for a phase change.
/// </summary>
public sealed class TriggerResult
{
    public HubRhythmPhase Phase { get; init; }
    public IReadOnlyList<TriggerAction> FiredActions { get; init; } = [];
}

/// <summary>
/// Fires trigger actions when the rhythm phase changes.
/// </summary>
public sealed class TriggerSystem
{
    private readonly Dictionary<HubRhythmPhase, PhaseTrigger> _triggers = new();
    private readonly HashSet<string> _firedTriggers = new();
    private readonly ILogger _logger;

    public TriggerSystem(ILogger logger) => _logger = logger;

    public void Register(PhaseTrigger trigger) =>
        _triggers[trigger.Phase] = trigger;

    /// <summary>
    /// Fire triggers for the given phase. Each trigger fires only once.
    /// Returns the actions that were fired, or empty if already fired / no trigger.
    /// </summary>
    public TriggerResult OnPhaseChanged(HubRhythmPhase phase)
    {
        if (!_triggers.TryGetValue(phase, out var trigger) || !_firedTriggers.Add(trigger.TriggerId))
            return new TriggerResult { Phase = phase };

        _logger.Info($"Trigger fired: {trigger.TriggerId} for phase {phase}");
        return new TriggerResult { Phase = phase, FiredActions = trigger.Actions };
    }

    public bool HasFired(string triggerId) => _firedTriggers.Contains(triggerId);
}
