using BabylonArchiveCore.Core.Scene;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Renders the current objective as an on-screen HUD element.
/// Tracks objective changes and provides formatted display text.
/// </summary>
public sealed class ObjectiveHUD
{
    private readonly ObjectiveTracker _tracker;
    private string? _lastObjectiveId;
    private string _displayText = "";
    private bool _hasChanged;

    public string DisplayText => _displayText;
    public bool HasChanged => _hasChanged;
    public string? CurrentObjectiveId => _tracker.GetActive()?.ObjectiveId;

    public ObjectiveHUD(ObjectiveTracker tracker)
    {
        _tracker = tracker;
        Refresh();
    }

    /// <summary>
    /// Update the HUD. Call once per frame.
    /// Sets <see cref="HasChanged"/> if the active objective changed.
    /// </summary>
    public void Update()
    {
        var current = _tracker.GetActive();
        var currentId = current?.ObjectiveId;

        _hasChanged = currentId != _lastObjectiveId;
        _lastObjectiveId = currentId;

        if (current is not null)
            _displayText = $"► {current.Text}";
        else
            _displayText = "";
    }

    /// <summary>
    /// Get a summary: completed / total objectives.
    /// </summary>
    public string GetProgress()
    {
        var completed = _tracker.GetCompleted().Count;
        var active = _tracker.GetActive();
        var total = completed + (active is not null ? 1 : 0);
        return $"{completed}/{total}";
    }

    private void Refresh()
    {
        var current = _tracker.GetActive();
        _lastObjectiveId = current?.ObjectiveId;
        _displayText = current is not null ? $"► {current.Text}" : "";
    }
}
