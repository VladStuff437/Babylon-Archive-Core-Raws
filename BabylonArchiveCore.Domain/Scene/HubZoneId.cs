namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Zones of Hub A-0 — the starting scene.
/// Frozen: Session 1, matches A0_Production_Bible.md
/// </summary>
public enum HubZoneId
{
    Capsule,
    Biometrics,
    Logistics,
    DroneNiche,
    Core,
    MissionTerminal,
    Research,
    ObservationGallery,
    HardArchiveEntrance,
    CommerceGateLocked,
    TechGateLocked,

    // Legacy alias — kept for backward compatibility with existing tests
    OperationTerminal = MissionTerminal,
}
