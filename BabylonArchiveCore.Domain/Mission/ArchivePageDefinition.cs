namespace BabylonArchiveCore.Domain.Mission;

public sealed class ArchivePageDefinition
{
    public string PageId { get; init; } = string.Empty;
    public string TomeId { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string MissionType { get; init; } = string.Empty;
    public bool IsMainPath { get; init; }
    public bool IsRepeatable { get; init; }
    public bool IsUnlockedInPlayerMode { get; init; }
    public bool IsAlwaysVisibleInAdminMode { get; init; }
    public IReadOnlyList<string> RequiredFlags { get; init; } = [];
    public IReadOnlyList<string> RewardFlags { get; init; } = [];
}
