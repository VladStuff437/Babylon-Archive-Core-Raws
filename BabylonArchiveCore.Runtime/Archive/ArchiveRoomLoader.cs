using BabylonArchiveCore.Domain.Archive;
using BabylonArchiveCore.Domain.World.Runtime;

namespace BabylonArchiveCore.Runtime.Archive;

public sealed class ArchiveRoomLoader
{
    public ArchiveRoomRuntimeState Load(string roomId, WorldRuntimeProfile profile)
    {
        return new ArchiveRoomRuntimeState
        {
            RoomId = roomId,
            IsLoaded = true,
            IsVisited = false,
            IsUnlockedInPlayerMode = string.Equals(roomId, "RM_HA_00_EntryOctagon", StringComparison.OrdinalIgnoreCase),
            IsAlwaysUnlockedInAdminMode = profile.UnlockHardArchivePreview,
            ConnectedRoomIds = ArchiveRoomGraph.GetConnections(roomId),
        };
    }
}
