namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Scene rhythm phases for Hub A-0: the wake-up sequence every operator follows.
/// Each phase unlocks new zones and interactions.
/// Frozen: Session 1, matches A0_Production_Bible.md
/// </summary>
public enum HubRhythmPhase
{
    /// <summary>Operator wakes up in the capsule. Only capsule exit is available.</summary>
    Awakening = 0,

    /// <summary>Operator must verify identity at Biometrics.</summary>
    Identification = 1,

    /// <summary>Operator picks up supplies at Logistics.</summary>
    Provisioning = 2,

    /// <summary>Operator activates the archive drone.</summary>
    DroneContact = 3,

    /// <summary>Operator activates C.O.R.E.</summary>
    Activation = 4,

    /// <summary>Mission terminal, research, gallery, and Hard-Archive view become available.</summary>
    OperationAccess = 5,

    // Legacy alias — for backward compatibility with existing code
    Supply = Provisioning,
}
