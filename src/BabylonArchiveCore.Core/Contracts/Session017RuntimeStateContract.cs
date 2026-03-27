namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S017: состояние runtime state manager.
/// </summary>
public sealed class Session017RuntimeStateContract
{
    public required string StateId { get; init; }

    public required string CurrentMode { get; init; }

    public required long LastAppliedTick { get; init; }

    public required bool IsDirty { get; init; }

    public string[]? RuntimeScopes { get; init; }
}
