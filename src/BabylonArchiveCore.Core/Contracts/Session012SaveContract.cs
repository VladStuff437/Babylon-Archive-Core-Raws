namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт данных S012: схема сейва v1.
/// </summary>
public sealed class Session012SaveContract
{
    public required string SaveId { get; init; }

    public required int SchemaVersion { get; init; }

    public required DateTimeOffset LastUpdatedUtc { get; init; }

    public required string PlayerState { get; init; }

    public required string WorldStateSnapshot { get; init; }

    public string[]? UnlockedArchiveAddresses { get; init; }
}
