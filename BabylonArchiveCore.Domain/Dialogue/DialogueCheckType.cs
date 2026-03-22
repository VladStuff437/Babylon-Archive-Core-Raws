namespace BabylonArchiveCore.Domain.Dialogue;

/// <summary>
/// Type of skill/knowledge check required for a dialogue option.
/// </summary>
public enum DialogueCheckType
{
    None,
    Deception,
    Evidence,
    Suspicion,
    Persuasion,
    Knowledge,
}
