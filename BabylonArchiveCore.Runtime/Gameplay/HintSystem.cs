using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Manages on-screen contextual hints ("Press [E]", "Go to biometrics").
/// Hints are registered per-object and shown when the player is in range.
/// </summary>
public sealed class HintSystem
{
    private readonly Dictionary<string, HintEntry> _hints = new();
    private HintEntry? _activeHint;

    public string? ActiveHintText => _activeHint?.Text;
    public string? ActiveHintObjectId => _activeHint?.ObjectId;

    /// <summary>Register a hint for an interactable object.</summary>
    public void Register(string objectId, string text)
    {
        _hints[objectId] = new HintEntry(objectId, text);
    }

    /// <summary>
    /// Update the active hint based on the nearest interactable.
    /// Pass the focused object ID (or null if nothing is in range).
    /// </summary>
    public void Update(string? focusedObjectId)
    {
        if (focusedObjectId is not null && _hints.TryGetValue(focusedObjectId, out var hint))
            _activeHint = hint;
        else
            _activeHint = null;
    }

    /// <summary>
    /// Format the active hint for display. Returns null if no hint active.
    /// </summary>
    public string? Format()
    {
        return _activeHint is not null ? $"[E] {_activeHint.Text}" : null;
    }

    /// <summary>Register all hints from a set of hub zones.</summary>
    public void RegisterFromZones(IEnumerable<HubZone> zones)
    {
        foreach (var zone in zones)
        foreach (var obj in zone.Objects)
        {
            if (!string.IsNullOrEmpty(obj.HintText))
                Register(obj.Id, obj.HintText);
        }
    }

    private sealed record HintEntry(string ObjectId, string Text);
}
