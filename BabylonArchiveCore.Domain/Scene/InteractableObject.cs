namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// An interactive object inside a hub zone.
/// </summary>
/// <summary>
/// Session 2: upgraded to 3D (Vec3 / Bounds3D). InteractionRadius for proximity checks.
/// </summary>
public sealed class InteractableObject
{
    public required string Id { get; init; }
    public required string DisplayName { get; init; }
    public required string HintText { get; init; }
    public required HubZoneId Zone { get; init; }
    public required Vec3 Position { get; init; }
    public required Bounds3D Bounds { get; init; }

    /// <summary>Max distance (metres) from which the player can interact.</summary>
    public float InteractionRadius { get; init; } = 1.5f;

    /// <summary>Minimum rhythm phase required for this object to be interactive.</summary>
    public required HubRhythmPhase RequiredPhase { get; init; }

    /// <summary>Behaviour type: trigger / terminal / npc / gate.</summary>
    public InteractiveType InteractiveType { get; init; } = InteractiveType.Trigger;

    /// <summary>Dialogue to play on interaction (if any).</summary>
    public string? DialogueId { get; init; }

    /// <summary>Items granted on interaction (item IDs).</summary>
    public IReadOnlyList<string>? GrantsItems { get; init; }

    /// <summary>Message shown when a gate is locked.</summary>
    public string? LockedMessage { get; init; }

    /// <summary>Context state: "locked", "ready", "active", "used", etc.</summary>
    public string ContextState { get; set; } = "ready";

    public bool IsActiveIn(HubRhythmPhase currentPhase) =>
        currentPhase >= RequiredPhase && ContextState != "used";
}
