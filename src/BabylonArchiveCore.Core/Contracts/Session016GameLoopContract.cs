namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S016: каркас game loop.
/// </summary>
public sealed class Session016GameLoopContract
{
    public required long Tick { get; init; }

    public required string LoopState { get; init; }

    public required int MaxUpdatesPerFrame { get; init; }

    public required bool IsPaused { get; init; }

    public double DeltaTimeSeconds { get; init; }
}
