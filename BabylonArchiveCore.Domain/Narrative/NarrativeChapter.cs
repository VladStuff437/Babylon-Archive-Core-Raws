namespace BabylonArchiveCore.Domain.Narrative;

/// <summary>
/// A chapter/beat in the narrative spine.
/// Each chapter requires certain flags/evidence levels to unlock and
/// advances the intervention storyline when triggered.
/// </summary>
public sealed class NarrativeChapter
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required int Order { get; init; }

    /// <summary>Minimum intervention certainty level required to trigger.</summary>
    public InterventionCertainty RequiredCertainty { get; init; } = InterventionCertainty.Unaware;

    /// <summary>WorldState flags that must be set before this chapter activates.</summary>
    public List<string> RequiredFlags { get; init; } = new();

    /// <summary>Flag set in WorldState when this chapter is triggered.</summary>
    public required string CompletionFlag { get; init; }

    /// <summary>Archive addresses that become accessible when this chapter triggers.</summary>
    public List<string> UnlocksAddresses { get; init; } = new();

    /// <summary>Brief narrative description shown to the player.</summary>
    public string? Synopsis { get; init; }
}
