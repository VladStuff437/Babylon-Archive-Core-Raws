namespace BabylonArchiveCore.Domain.Mission;

public sealed class TerminalMissionBoardSlot
{
    public string SlotId { get; init; } = string.Empty;
    public string? PageId { get; init; }
    public string DisplayTitle { get; init; } = string.Empty;
    public string DisplayStatus { get; init; } = string.Empty;
    public bool IsSelectableInPlayerMode { get; init; }
    public bool IsSelectableInAdminMode { get; init; }
}
