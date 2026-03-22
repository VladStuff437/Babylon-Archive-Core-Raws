namespace BabylonArchiveCore.Domain.Player;

/// <summary>
/// A single item in the player's inventory.
/// </summary>
public sealed class InventoryItem
{
    public required string ItemId { get; init; }
    public required string Name { get; init; }
    public required ItemType Type { get; init; }
}

/// <summary>
/// Classification of inventory items.
/// </summary>
public enum ItemType
{
    KeyItem = 0,
    Tool = 1,
    Consumable = 2,
    Misc = 3,
}
