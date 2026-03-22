namespace BabylonArchiveCore.Domain.Mission;

public sealed class PageGenerationContext
{
    public string PageId { get; init; } = string.Empty;
    public int Seed { get; init; }
    public string MainTheme { get; init; } = string.Empty;
    public string MissionType { get; init; } = string.Empty;
    public IReadOnlyList<string> RequiredFlags { get; init; } = [];
}
