namespace BabylonArchiveCore.Domain.Mission;

public sealed class GeneratedMissionPage
{
    public string PageId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<string> Nodes { get; init; } = [];
    public IReadOnlyList<string> Rewards { get; init; } = [];
    public IReadOnlyList<string> Consequences { get; init; } = [];
}
