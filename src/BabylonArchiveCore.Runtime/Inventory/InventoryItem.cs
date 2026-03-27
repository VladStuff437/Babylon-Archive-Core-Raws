namespace BabylonArchiveCore.Runtime.Inventory;

/// <summary>
/// Одна запись инвентаря.
/// </summary>
public sealed class InventoryItem
{
    public required string ItemId { get; init; }
    public required string Name { get; init; }
    public int Quantity { get; set; } = 1;
    public int MaxStack { get; init; } = 99;
}
