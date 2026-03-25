namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Canonical ids for contour-stage 3D models used in Session 10 desktop rendering.
/// These ids are data-facing and should remain stable across content revisions.
/// </summary>
public static class ContourModelIds
{
    // Reserved for future playable character contour rendering; A0 currently uses whitebox player visuals.
    public const string AlanArcwain = "CHR_ALAN_ARCWAIN_CONTOUR";

    public const string CapsuleExit = "PRP_CAPSULE_EXIT_CONTOUR";
    public const string BioScanner = "PRP_BIO_SCANNER_CONTOUR";
    public const string SupplyTerminal = "PRP_SUPPLY_TERMINAL_CONTOUR";
    public const string DroneDock = "PRP_DRONE_DOCK_CONTOUR";
    public const string CoreConsole = "PRP_CORE_CONSOLE_CONTOUR";
    public const string MissionTerminal = "PRP_MISSION_TERMINAL_CONTOUR";
    public const string ResearchTerminal = "PRP_RESEARCH_TERMINAL_CONTOUR";
    public const string GalleryOverlook = "PRP_GALLERY_OVERLOOK_CONTOUR";
    public const string ArchiveGate = "PRP_ARCHIVE_GATE_CONTOUR";

    public const string MissionBoard = "PRP_MISSION_BOARD_CONTOUR";
    public const string ResearchLab = "PRP_RESEARCH_LAB_CONTOUR";
    public const string ToolBench = "PRP_TOOL_BENCH_CONTOUR";
    public const string ArchiveControl = "PRP_ARCHIVE_CONTROL_CONTOUR";
    public const string CommerceDesk = "PRP_COMMERCE_DESK_CONTOUR";

    public const string ArchiveCorridor = "ENV_ARCHIVE_CORRIDOR_CONTOUR";
    public const string EntryOctagon = "ENV_ENTRY_OCTAGON_CONTOUR";
    public const string IndexVestibule = "ENV_INDEX_VESTIBULE_CONTOUR";
    public const string ResearchRoom01 = "ENV_RESEARCH_ROOM_01_CONTOUR";
    public const string StackRingPreview = "ENV_STACK_RING_PREVIEW_CONTOUR";
    public const string CommerceHall = "ENV_COMMERCE_HALL_CONTOUR";
    public const string TechHall = "ENV_TECH_HALL_CONTOUR";
    public const string ArchivePreview = "ENV_ARCHIVE_PREVIEW_CONTOUR";
}
