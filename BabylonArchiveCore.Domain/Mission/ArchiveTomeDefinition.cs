namespace BabylonArchiveCore.Domain.Mission;

public sealed class ArchiveTomeDefinition
{
    public string TomeId { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string ZoneOrRoomId { get; init; } = string.Empty;
    public IReadOnlyList<ArchivePageDefinition> Pages { get; init; } = [];
    public bool IsStoryCritical { get; init; }
    public bool IsVisibleInMissionTerminal { get; init; }
    public bool IsVisibleInHardArchivePhysicalMode { get; init; }
}
