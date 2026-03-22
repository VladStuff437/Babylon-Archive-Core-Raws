using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Narrative;
using BabylonArchiveCore.Domain.World;

namespace BabylonArchiveCore.Runtime.Narrative;

/// <summary>
/// Drives the narrative spine: checks which chapters should trigger
/// based on certainty + flags, activates them, and opens archive addresses.
/// </summary>
public sealed class NarrativeSpineRunner
{
    private readonly List<NarrativeChapter> _chapters;
    private readonly WorldState _worldState;
    private readonly ILogger _logger;
    private readonly HashSet<string> _triggeredChapterIds = new();

    public IReadOnlySet<string> TriggeredChapters => _triggeredChapterIds;

    public NarrativeSpineRunner(
        List<NarrativeChapter> chapters,
        WorldState worldState,
        ILogger logger)
    {
        _chapters = chapters.OrderBy(c => c.Order).ToList();
        _worldState = worldState;
        _logger = logger;
    }

    /// <summary>
    /// Evaluate all chapters against the current WorldState and intervention certainty.
    /// Returns newly triggered chapters in this call.
    /// </summary>
    public List<NarrativeChapter> Evaluate(InterventionCertainty certainty)
    {
        var newlyTriggered = new List<NarrativeChapter>();

        foreach (var chapter in _chapters)
        {
            if (_triggeredChapterIds.Contains(chapter.Id))
                continue;

            if ((int)certainty < (int)chapter.RequiredCertainty)
                continue;

            if (!chapter.RequiredFlags.All(f => _worldState.HasFlag(f)))
                continue;

            // Trigger this chapter
            _triggeredChapterIds.Add(chapter.Id);
            _worldState.SetFlag(chapter.CompletionFlag);

            foreach (var addr in chapter.UnlocksAddresses)
                _worldState.VisitedAddresses.Add(addr);

            _logger.Info($"Narrative: chapter '{chapter.Title}' (#{chapter.Order}) triggered. " +
                         $"Flag '{chapter.CompletionFlag}' set, {chapter.UnlocksAddresses.Count} address(es) unlocked.");

            newlyTriggered.Add(chapter);
        }

        return newlyTriggered;
    }
}
