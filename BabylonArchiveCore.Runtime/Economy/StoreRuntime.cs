using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Economy;

namespace BabylonArchiveCore.Runtime.Economy;

/// <summary>
/// Store runtime: manages catalog, inventory, and purchases with pay-to-win prevention.
/// </summary>
public sealed class StoreRuntime
{
    private readonly Dictionary<string, StoreItemDefinition> _catalog = new();
    private readonly Dictionary<string, int> _inventory = new(); // itemId → owned count
    private readonly Wallet _wallet;
    private readonly EventBus _eventBus;
    private readonly ILogger _logger;

    public StoreRuntime(Wallet wallet, EventBus eventBus, ILogger logger)
    {
        _wallet = wallet;
        _eventBus = eventBus;
        _logger = logger;
    }

    public IReadOnlyDictionary<string, StoreItemDefinition> Catalog => _catalog;
    public IReadOnlyDictionary<string, int> Inventory => _inventory;

    /// <summary>
    /// Registers an item in the store catalog. Rejects pay-to-win items.
    /// </summary>
    public bool RegisterItem(StoreItemDefinition item)
    {
        if (!item.IsPayToWinCompliant)
        {
            _logger.Warn($"Rejected item '{item.Id}': Supply items cannot be sold for Launs (pay-to-win).");
            return false;
        }
        _catalog[item.Id] = item;
        return true;
    }

    /// <summary>
    /// Attempts to purchase an item using the specified currency.
    /// </summary>
    public PurchaseResult Purchase(string itemId, CurrencyType currency, int operatorLevel)
    {
        if (!_catalog.TryGetValue(itemId, out var item))
            return PurchaseResult.Fail("Item not found in catalog.");

        if (operatorLevel < item.RequiredLevel)
            return PurchaseResult.Fail($"Requires level {item.RequiredLevel}, operator is level {operatorLevel}.");

        // Check max ownership
        if (item.MaxOwned >= 0)
        {
            var owned = _inventory.GetValueOrDefault(itemId);
            if (owned >= item.MaxOwned)
                return PurchaseResult.Fail($"Already own maximum ({item.MaxOwned}) of this item.");
        }

        // Pay-to-win enforcement: Supply items cannot use Launs
        if (currency == CurrencyType.Launs && item.Category == StoreItemCategory.Supply)
            return PurchaseResult.Fail("Supply items cannot be purchased with Launs (no pay-to-win).");

        // Determine price
        var price = currency switch
        {
            CurrencyType.Credits => item.CreditPrice,
            CurrencyType.Launs => item.LaunPrice,
            _ => 0,
        };

        if (price <= 0)
            return PurchaseResult.Fail($"Item '{item.Name}' is not available for {currency}.");

        // Charge
        if (!_wallet.TrySpend(currency, price))
            return PurchaseResult.Fail($"Insufficient {currency}: need {price}, have {_wallet.GetBalance(currency)}.");

        // Add to inventory
        _inventory.TryGetValue(itemId, out var count);
        _inventory[itemId] = count + 1;

        _logger.Info($"Purchased '{item.Name}' for {price} {currency}. Inventory: {_inventory[itemId]}x.");

        _eventBus.Publish(new PurchaseCompletedEvent
        {
            ItemId = itemId,
            CurrencyUsed = currency,
            AmountCharged = price,
        });

        return PurchaseResult.Ok(itemId, currency, price);
    }

    public int GetOwnedCount(string itemId) => _inventory.GetValueOrDefault(itemId);
}
