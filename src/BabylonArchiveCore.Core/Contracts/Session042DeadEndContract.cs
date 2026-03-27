namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S042 contract: dead-end validation results.
/// </summary>
public sealed class Session042DeadEndContract
{
    public int ContractVersion { get; init; } = 42;

    public required string MissionId { get; init; }

    public required string[] DeadEndNodeIds { get; init; }

    public bool AllowsFallback { get; init; } = true;
}
