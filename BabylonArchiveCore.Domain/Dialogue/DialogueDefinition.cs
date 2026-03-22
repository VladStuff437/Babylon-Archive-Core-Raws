namespace BabylonArchiveCore.Domain.Dialogue;

/// <summary>
/// A complete dialogue tree: metadata + all lines keyed by id.
/// </summary>
public sealed class DialogueDefinition
{
    public required string Id { get; init; }
    public required string SpeakerEntity { get; init; }
    public required string StartLineId { get; init; }

    /// <summary>All dialogue lines keyed by line id.</summary>
    public Dictionary<string, DialogueLine> Lines { get; init; } = new();
}
