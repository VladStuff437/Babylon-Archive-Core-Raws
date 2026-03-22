namespace BabylonArchiveCore.Domain.Mission;

public sealed class LaunchRouteProfile
{
    public LaunchRouteType RouteType { get; init; }
    public string SourceNodeId { get; init; } = string.Empty;
    public string TargetNodeId { get; init; } = string.Empty;
    public string? PageId { get; init; }
}
