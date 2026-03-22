using BabylonArchiveCore.Domain.Mission;

namespace BabylonArchiveCore.Runtime.Mission;

public static class Session10MissionCatalog
{
    public static IReadOnlyList<ArchivePageDefinition> BuildInitialPages()
    {
        return
        [
            new ArchivePageDefinition
            {
                PageId = "P01_ECHO_STACK_09",
                TomeId = "T01_ThresholdOperations",
                DisplayName = "Эхо в стеке 9",
                MissionType = "Investigation",
                IsMainPath = true,
                IsRepeatable = true,
                IsUnlockedInPlayerMode = true,
                IsAlwaysVisibleInAdminMode = true,
                RequiredFlags = ["protocol_zero"],
                RewardFlags = ["PAGE_P01_COMPLETE"],
            },
            new ArchivePageDefinition
            {
                PageId = "P02_INCOMPATIBLE_INDEX",
                TomeId = "T01_ThresholdOperations",
                DisplayName = "Несовместимый индекс",
                MissionType = "ResearchPuzzle",
                IsMainPath = true,
                IsRepeatable = true,
                IsUnlockedInPlayerMode = false,
                IsAlwaysVisibleInAdminMode = true,
                RequiredFlags = ["PAGE_P01_COMPLETE"],
                RewardFlags = ["PAGE_P02_COMPLETE"],
            },
            new ArchivePageDefinition
            {
                PageId = "P03_EMPTY_WITNESS",
                TomeId = "T02_InterferenceTraces",
                DisplayName = "Пустой свидетель",
                MissionType = "DialogueAnomaly",
                IsMainPath = true,
                IsRepeatable = true,
                IsUnlockedInPlayerMode = false,
                IsAlwaysVisibleInAdminMode = true,
                RequiredFlags = ["PAGE_P02_COMPLETE"],
                RewardFlags = ["PAGE_P03_COMPLETE"],
            },
        ];
    }

    public static IReadOnlyList<TerminalMissionBoardSlot> BuildInitialBoard(bool includeAdminDebugSlot)
    {
        var slots = new List<TerminalMissionBoardSlot>
        {
            new()
            {
                SlotId = "slot_p01",
                PageId = "P01_ECHO_STACK_09",
                DisplayTitle = "P01 // Эхо в стеке 9",
                DisplayStatus = "INDEXED",
                IsSelectableInPlayerMode = true,
                IsSelectableInAdminMode = true,
            },
            new()
            {
                SlotId = "slot_p02",
                PageId = "P02_INCOMPATIBLE_INDEX",
                DisplayTitle = "P02 // Несовместимый индекс",
                DisplayStatus = "LOCKED",
                IsSelectableInPlayerMode = false,
                IsSelectableInAdminMode = true,
            },
            new()
            {
                SlotId = "slot_p03",
                PageId = "P03_EMPTY_WITNESS",
                DisplayTitle = "P03 // Пустой свидетель",
                DisplayStatus = "SHADOWED",
                IsSelectableInPlayerMode = false,
                IsSelectableInAdminMode = true,
            },
        };

        if (includeAdminDebugSlot)
        {
            slots.Add(new TerminalMissionBoardSlot
            {
                SlotId = "slot_debug_generator",
                PageId = null,
                DisplayTitle = "DEBUG // Generated Preview",
                DisplayStatus = "ADMIN",
                IsSelectableInPlayerMode = false,
                IsSelectableInAdminMode = true,
            });
        }

        return slots;
    }

    public static IReadOnlyList<string> InitialArchivePreviewRooms =>
    [
        "RM_HA_00_EntryOctagon",
        "RM_HA_01_IndexVestibule",
        "RM_HA_02_StackRing",
        "RM_HA_03_FirstShelfCluster",
        "RM_HA_04_TutorialTomeCell",
        "RM_HA_05_ReturnNode",
        "RM_HA_07_AnomalyAlcove",
        "RM_HA_08_ObservationShaft",
        "RM_HA_09_LowerLiftNode",
    ];
}
