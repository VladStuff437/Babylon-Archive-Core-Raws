namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Type of interactive object. Determines interaction behaviour.
/// Matches Production Bible §4 column "Тип".
/// </summary>
public enum InteractiveType
{
    /// <summary>Instant one-shot action (e.g. capsule exit).</summary>
    Trigger = 0,

    /// <summary>Opens a terminal screen with content (e.g. bio scanner, CORE console).</summary>
    Terminal = 1,

    /// <summary>NPC dialogue encounter (e.g. drone dock).</summary>
    Npc = 2,

    /// <summary>Locked gate with blocked message (e.g. archive entrance).</summary>
    Gate = 3,
}
