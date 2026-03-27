namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S028: тик-режим автоатаки.
/// </summary>
public sealed class Session028AutoAttackContract
{
    public required string ActorId { get; init; }

    public string? TargetId { get; init; }

    public bool IsActive { get; init; }

    public int AttackIntervalTicks { get; init; }

    public bool IsMissionTransitionLocked { get; init; }
}
