namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S027: боевой ввод Tab/Esc с защитой миссионных переходов.
/// </summary>
public sealed class Session027CombatInputContract
{
    public required string ActorId { get; init; }

    public required string[] TargetQueue { get; init; }

    public string? CurrentTargetId { get; init; }

    public bool IsTargetingEnabled { get; init; }

    public bool IsMissionTransitionLocked { get; init; }
}
