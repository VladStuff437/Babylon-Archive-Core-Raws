namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт данных S013: политика версионирования схем.
/// </summary>
public sealed class Session013SchemaVersionContract
{
    public required int CurrentVersion { get; init; }

    public required int MinimumCompatibleVersion { get; init; }

    public required string Channel { get; init; }

    public required string MigrationPolicy { get; init; }

    public string[]? SupportedLegacyVersions { get; init; }
}
