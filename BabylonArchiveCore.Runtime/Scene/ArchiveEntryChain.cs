namespace BabylonArchiveCore.Runtime.Scene;

public static class ArchiveEntryChain
{
    public const string ArchiveControl = "archive_control";
    public const string ArchiveCorridor = "archive_corridor";
    public const string EntryOctagon = "entry_octagon";
    public const string IndexVestibule = "index_vestibule";
    public const string ResearchRoom01 = "research_room_01";
    public const string StackRingPreview = "stack_ring_preview";

    public static readonly IReadOnlyList<string> OrderedNodes =
    [
        ArchiveControl,
        ArchiveCorridor,
        EntryOctagon,
        IndexVestibule,
        ResearchRoom01,
        StackRingPreview,
    ];

    public static int GetCompletedSteps(
        bool hardArchivePartialUnlock,
        bool entryOctagonUnlocked,
        bool indexVestibuleUnlocked,
        bool researchRoomUnlocked,
        bool stackRingPreviewUnlocked)
    {
        var completed = 1; // archive_control is the root of the chain in OperationAccess

        if (hardArchivePartialUnlock)
            completed = 2;
        if (entryOctagonUnlocked)
            completed = 3;
        if (indexVestibuleUnlocked)
            completed = 4;
        if (researchRoomUnlocked)
            completed = 5;
        if (stackRingPreviewUnlocked)
            completed = 6;

        return completed;
    }

    public static string? GetNextNodeId(
        bool hardArchivePartialUnlock,
        bool entryOctagonUnlocked,
        bool indexVestibuleUnlocked,
        bool researchRoomUnlocked,
        bool stackRingPreviewUnlocked)
    {
        if (!hardArchivePartialUnlock)
            return ArchiveCorridor;
        if (!entryOctagonUnlocked)
            return EntryOctagon;
        if (!indexVestibuleUnlocked)
            return IndexVestibule;
        if (!researchRoomUnlocked)
            return ResearchRoom01;
        if (!stackRingPreviewUnlocked)
            return StackRingPreview;

        return null;
    }
}
