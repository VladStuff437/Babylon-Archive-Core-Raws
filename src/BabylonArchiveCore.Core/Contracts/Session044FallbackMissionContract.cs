namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S044 contract: deterministic fallback mission routing.
/// </summary>
public sealed class Session044FallbackMissionContract
{
    public int ContractVersion { get; init; } = 44;

    public required string SourceMissionId { get; init; }

    public required string FallbackMissionId { get; init; }

    public required string[] ReasonCodes { get; init; }

    public bool IsDeterministic { get; init; } = true;
}
