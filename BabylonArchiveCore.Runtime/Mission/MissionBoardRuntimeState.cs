using BabylonArchiveCore.Domain.Mission;

namespace BabylonArchiveCore.Runtime.Mission;

public sealed class MissionBoardRuntimeState
{
    public IReadOnlyList<TerminalMissionBoardSlot> VisibleSlots { get; init; } = [];
    public string? SelectedSlotId { get; set; }
    public string CurrentTomeId { get; set; } = string.Empty;
    public bool IsAdminViewEnabled { get; set; }
}
