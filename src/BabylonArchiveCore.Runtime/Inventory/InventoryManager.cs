namespace BabylonArchiveCore.Runtime.Inventory;

/// <summary>
/// Менеджер инвентаря с проверкой лимитов и стека.
/// </summary>
public sealed class InventoryManager
{
    private readonly List<InventoryItem> _items = new();

    public int Capacity { get; }

    public InventoryManager(int capacity = 20)
    {
        Capacity = capacity;
    }

    public IReadOnlyList<InventoryItem> Items => _items.AsReadOnly();

    public bool TryAdd(string itemId, string name, int quantity = 1, int maxStack = 99)
    {
        var existing = _items.Find(i => i.ItemId == itemId);
        if (existing is not null)
        {
            int canAdd = Math.Min(quantity, existing.MaxStack - existing.Quantity);
            if (canAdd <= 0) return false;
            existing.Quantity += canAdd;
            return true;
        }
        if (_items.Count >= Capacity) return false;
        _items.Add(new InventoryItem { ItemId = itemId, Name = name, Quantity = Math.Min(quantity, maxStack), MaxStack = maxStack });
        return true;
    }

    public bool TryRemove(string itemId, int quantity = 1)
    {
        var existing = _items.Find(i => i.ItemId == itemId);
        if (existing is null || existing.Quantity < quantity) return false;
        existing.Quantity -= quantity;
        if (existing.Quantity <= 0) _items.Remove(existing);
        return true;
    }

    public InventoryItem? Find(string itemId) => _items.Find(i => i.ItemId == itemId);
}
