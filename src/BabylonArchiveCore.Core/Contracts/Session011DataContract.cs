namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт данных S011: базовая схема игровых данных v1.
/// </summary>
public sealed class Session011DataContract
{
    public required string SaveId { get; init; }

    public required int SchemaVersion { get; init; }

    public required long WorldSeed { get; init; }

    public required string CurrentMode { get; init; }

    public long WorldTick { get; init; }

    public string? ActiveMissionId { get; init; }

    public Dictionary<string, string>? Metadata { get; init; }
}
