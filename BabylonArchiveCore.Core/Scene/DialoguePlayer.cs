using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Core.Scene;

/// <summary>
/// Plays an inline dialogue sequence line-by-line.
/// Call Start() then Advance() to step through.
/// </summary>
public sealed class DialoguePlayer
{
    private readonly Dictionary<string, InlineDialogue> _dialogues = new();
    private InlineDialogue? _current;
    private int _lineIndex = -1;

    public bool IsPlaying => _current is not null && !IsComplete;
    public bool IsComplete => _current is not null && _lineIndex >= _current.Lines.Count;
    public string? CurrentDialogueId => _current?.DialogueId;

    public InlineDialogueLine? CurrentLine =>
        _current is not null && _lineIndex >= 0 && _lineIndex < _current.Lines.Count
            ? _current.Lines[_lineIndex]
            : null;

    public void Register(InlineDialogue dialogue) =>
        _dialogues[dialogue.DialogueId] = dialogue;

    /// <summary>Start playing a dialogue. Returns false if dialogue not found.</summary>
    public bool Start(string dialogueId)
    {
        if (!_dialogues.TryGetValue(dialogueId, out var dlg))
            return false;

        _current = dlg;
        _lineIndex = 0;
        return true;
    }

    /// <summary>Advance to the next line. Returns the new current line, or null if finished.</summary>
    public InlineDialogueLine? Advance()
    {
        if (_current is null) return null;
        _lineIndex++;
        return CurrentLine;
    }

    /// <summary>Stop the current dialogue immediately.</summary>
    public void Stop()
    {
        _current = null;
        _lineIndex = -1;
    }

    /// <summary>Get all lines from a completed or in-progress dialogue as transcript.</summary>
    public IReadOnlyList<InlineDialogueLine> GetTranscript(string dialogueId) =>
        _dialogues.TryGetValue(dialogueId, out var dlg) ? dlg.Lines : [];
}
