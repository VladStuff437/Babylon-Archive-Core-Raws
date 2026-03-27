namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S015: таксономия runtime-событий.
/// </summary>
public sealed class Session015EventTaxonomyContract
{
    public required string EventName { get; init; }

    public required string Domain { get; init; }

    public required string Category { get; init; }

    public required string Severity { get; init; }

    public string[]? Tags { get; init; }
}
