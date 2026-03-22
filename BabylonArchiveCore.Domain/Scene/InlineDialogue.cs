namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// A single inline dialogue line (speaker + text).
/// </summary>
public sealed class InlineDialogueLine
{
    public required string LineId { get; init; }
    public required string Speaker { get; init; }
    public required string Text { get; init; }
    public float Delay { get; init; } = 1.5f;
}

/// <summary>
/// A self-contained inline dialogue sequence (non-branching).
/// </summary>
public sealed class InlineDialogue
{
    public required string DialogueId { get; init; }
    public required string SpeakerDisplayName { get; init; }
    public required IReadOnlyList<InlineDialogueLine> Lines { get; init; }
}
