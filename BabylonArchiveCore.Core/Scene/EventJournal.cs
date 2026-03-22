namespace BabylonArchiveCore.Core.Scene;

/// <summary>
/// Chronological in-game event journal. Records player actions, scene events,
/// and system messages for the player to review.
/// </summary>
public sealed class EventJournal
{
    private readonly List<JournalEntry> _entries = new();

    public IReadOnlyList<JournalEntry> Entries => _entries;

    public void Record(string category, string message)
    {
        _entries.Add(new JournalEntry(DateTime.UtcNow, category, message));
    }

    public IReadOnlyList<JournalEntry> GetByCategory(string category) =>
        _entries.Where(e => e.Category == category).ToList();

    public JournalEntry? Latest => _entries.Count > 0 ? _entries[^1] : null;
}

public sealed record JournalEntry(DateTime TimestampUtc, string Category, string Message);
