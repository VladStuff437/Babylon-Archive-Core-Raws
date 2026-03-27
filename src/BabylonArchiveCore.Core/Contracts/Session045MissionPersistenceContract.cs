namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S045 contract: mission runtime snapshot persistence metadata.
/// </summary>
public sealed class Session045MissionPersistenceContract
{
    public int ContractVersion { get; init; } = 45;

    public required string MissionId { get; init; }

    public int SnapshotVersion { get; init; } = 45;

    public required string StateChecksum { get; init; }

    public int StepCount { get; init; }
}
