namespace BabylonArchiveCore.Runtime.Archive;

public static class ArchiveRoomGraph
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> Map =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["RM_HA_00_EntryOctagon"] = ["RM_HA_01_IndexVestibule", "RM_HA_02_StackRing", "RM_HA_05_ReturnNode"],
            ["RM_HA_01_IndexVestibule"] = ["RM_HA_03_FirstShelfCluster", "RM_HA_04_TutorialTomeCell"],
            ["RM_HA_02_StackRing"] = ["RM_HA_07_AnomalyAlcove", "RM_HA_08_ObservationShaft"],
            ["RM_HA_03_FirstShelfCluster"] = ["RM_HA_04_TutorialTomeCell"],
            ["RM_HA_04_TutorialTomeCell"] = ["RM_HA_05_ReturnNode"],
            ["RM_HA_05_ReturnNode"] = ["RM_A0_11_HardArchiveThreshold"],
            ["RM_HA_07_AnomalyAlcove"] = ["RM_HA_02_StackRing"],
            ["RM_HA_08_ObservationShaft"] = ["RM_HA_09_LowerLiftNode"],
            ["RM_HA_09_LowerLiftNode"] = [],
        };

    public static IReadOnlyList<string> GetConnections(string roomId)
        => Map.TryGetValue(roomId, out var links) ? links : [];
}
