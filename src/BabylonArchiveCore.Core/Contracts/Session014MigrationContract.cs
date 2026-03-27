namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S014: настройки миграции данных.
/// </summary>
public sealed class Session014MigrationContract
{
    public required int SourceVersion { get; init; }

    public required int TargetVersion { get; init; }

    public required string MigrationId { get; init; }

    public required bool IsBackwardCompatible { get; init; }

    public string[]? AppliedSteps { get; init; }
}
