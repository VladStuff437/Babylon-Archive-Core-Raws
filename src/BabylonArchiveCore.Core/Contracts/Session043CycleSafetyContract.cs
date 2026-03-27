namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S043 contract: unsafe cycle validation data.
/// </summary>
public sealed class Session043CycleSafetyContract
{
    public int ContractVersion { get; init; } = 43;

    public required string MissionId { get; init; }

    public required string[] UnsafeCycleNodeIds { get; init; }

    public int MaxAllowedCycleLength { get; init; } = 8;
}
