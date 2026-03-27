namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S041 contract: mission reachability validation inputs.
/// </summary>
public sealed class Session041ReachabilityContract
{
    public int ContractVersion { get; init; } = 41;

    public required string MissionId { get; init; }

    public required string StartNodeId { get; init; }

    public required string[] NodeIds { get; init; }
}
