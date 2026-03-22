namespace BabylonArchiveCore.Domain.Mission;

/// <summary>
/// Classification of a mission.
/// </summary>
public enum MissionType
{
    Main,
    Side,
    Repeatable,
    Timed,
    Puzzle,
    Dialogue,
    Combat,
    HardChoice,
    Competition,
}
