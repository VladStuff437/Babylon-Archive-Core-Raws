namespace BabylonArchiveCore.Domain.Archive;

public sealed class ArchiveRoomRuntimeState
{
    public string RoomId { get; init; } = string.Empty;
    public bool IsLoaded { get; set; }
    public bool IsVisited { get; set; }
    public bool IsUnlockedInPlayerMode { get; set; }
    public bool IsAlwaysUnlockedInAdminMode { get; set; }
    public IReadOnlyList<string> ConnectedRoomIds { get; init; } = [];
}
