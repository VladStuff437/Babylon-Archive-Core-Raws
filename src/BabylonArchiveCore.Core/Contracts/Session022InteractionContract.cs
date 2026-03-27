namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S022: геймплей-контур взаимодействий, инвентаря, боя.
/// </summary>
public sealed class Session022InteractionContract
{
    public required string InteractionType { get; init; } // Pickup, Talk, Use, Examine
    public required string TargetId { get; init; }
    public required string ActorId { get; init; }
    public bool Consumed { get; init; }
}
