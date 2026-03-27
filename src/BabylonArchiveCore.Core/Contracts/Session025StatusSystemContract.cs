namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S025: состояние статус-системы.
/// </summary>
public sealed class Session025StatusSystemContract
{
    public required string OwnerId { get; init; }
    public required StatusState[] Statuses { get; init; }
}

public sealed class StatusState
{
    public required string EffectId { get; init; }
    public required string Category { get; init; }
    public required int RemainingTicks { get; init; }
    public required int StackCount { get; init; }
}
