namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S023: инвентарь v1 — консолидация инвентарных операций.
/// </summary>
public sealed class Session023InventoryContract
{
    public required string OwnerId { get; init; }
    public required int Capacity { get; init; }
    public required InventorySlot[] Slots { get; init; }
}

public sealed class InventorySlot
{
    public required string ItemId { get; init; }
    public required string Name { get; init; }
    public int Quantity { get; set; } = 1;
    public int MaxStack { get; init; } = 99;
    public string? Category { get; init; }
}
