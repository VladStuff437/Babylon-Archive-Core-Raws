using BabylonArchiveCore.Domain.Mission;

namespace BabylonArchiveCore.Domain.Dialogue;

/// <summary>
/// A single selectable choice in a dialogue line.
/// May require a skill check (type + difficulty) and applies effects on selection.
/// </summary>
public sealed class DialogueOption
{
    public required string Id { get; init; }
    public required string Text { get; init; }

    /// <summary>Target line id to jump to after selecting this option.</summary>
    public required string TargetLineId { get; init; }

    /// <summary>Check type required; None = always available.</summary>
    public DialogueCheckType CheckType { get; init; } = DialogueCheckType.None;

    /// <summary>Difficulty threshold (0-100). Player stat ≥ difficulty = pass.</summary>
    public int Difficulty { get; init; }

    /// <summary>Flag required to unlock this option (null = no requirement).</summary>
    public string? RequiredFlag { get; init; }

    /// <summary>Effects applied to WorldState when this option is chosen.</summary>
    public MissionEffect? Effect { get; init; }
}
