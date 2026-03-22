namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Defines a single trigger action to fire.
/// </summary>
public sealed class TriggerAction
{
    public required TriggerActionType Type { get; init; }
    public string? ObjectiveId { get; init; }
    public string? DialogueId { get; init; }
    public string? Text { get; init; }
}

public enum TriggerActionType
{
    SetObjective = 0,
    PlayDialogue = 1,
    JournalEntry = 2,
}

/// <summary>
/// A phase-change trigger definition with condition and actions.
/// </summary>
public sealed class PhaseTrigger
{
    public required string TriggerId { get; init; }
    public required HubRhythmPhase Phase { get; init; }
    public required IReadOnlyList<TriggerAction> Actions { get; init; }
}
