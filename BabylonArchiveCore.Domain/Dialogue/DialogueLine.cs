namespace BabylonArchiveCore.Domain.Dialogue;

/// <summary>
/// A single unit of dialogue: a speaker says text, then the player picks from options.
/// Terminal lines have no options and end the conversation.
/// </summary>
public sealed class DialogueLine
{
    public required string Id { get; init; }
    public required string Speaker { get; init; }
    public required string Text { get; init; }

    /// <summary>Available player responses. Empty = terminal line (dialogue ends).</summary>
    public List<DialogueOption> Options { get; init; } = new();

    /// <summary>If true, this line ends the dialogue.</summary>
    public bool IsTerminal => Options.Count == 0;
}
