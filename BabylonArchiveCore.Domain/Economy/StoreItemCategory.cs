namespace BabylonArchiveCore.Domain.Economy;

/// <summary>
/// Category of a store item. Enforces pay-to-win restriction:
/// only Convenience and Cosmetic items can be sold for Launs.
/// </summary>
public enum StoreItemCategory
{
    /// <summary>Consumable or utility item purchasable with Credits.</summary>
    Supply,

    /// <summary>Convenience item: saves time, no exclusive power. Can be sold for Launs.</summary>
    Convenience,

    /// <summary>Cosmetic item: visual only, no gameplay advantage. Can be sold for Launs.</summary>
    Cosmetic,

    /// <summary>Expansion content: new zones/tomes, purchasable with Credits or Launs.</summary>
    Expansion,
}
