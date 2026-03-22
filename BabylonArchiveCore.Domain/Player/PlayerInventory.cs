namespace BabylonArchiveCore.Domain.Player;

/// <summary>
/// Tracks the operator's inventory (key items, tools, consumables).
/// </summary>
public sealed class PlayerInventory
{
    private readonly List<InventoryItem> _items = new();

    public IReadOnlyList<InventoryItem> Items => _items;

    public void Add(InventoryItem item)
    {
        if (!Has(item.ItemId))
            _items.Add(item);
    }

    public bool Remove(string itemId) =>
        _items.RemoveAll(i => i.ItemId == itemId) > 0;

    public bool Has(string itemId) =>
        _items.Any(i => i.ItemId == itemId);

    public IReadOnlyList<InventoryItem> GetAll() => _items;
}
