namespace BabylonArchiveCore.Domain.Mission;

public sealed class MissionLaunchValidationResult
{
    public bool CanLaunch { get; init; }
    public string? BlockReason { get; init; }
    public bool IsAdminOverride { get; init; }
    public string LaunchMode { get; init; } = string.Empty;
}
