namespace BabylonArchiveCore.Domain.Economy;

/// <summary>
/// A store item definition. Items with category Supply can only be purchased
/// with Credits. Convenience/Cosmetic/Expansion can also use Launs.
/// </summary>
public sealed class StoreItemDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required StoreItemCategory Category { get; init; }

    /// <summary>Price in Credits (0 means not available for Credits).</summary>
    public int CreditPrice { get; init; }

    /// <summary>Price in Launs (0 means not available for Launs).</summary>
    public int LaunPrice { get; init; }

    /// <summary>Maximum quantity the player can own (-1 = unlimited).</summary>
    public int MaxOwned { get; init; } = -1;

    /// <summary>Minimum operator level to purchase.</summary>
    public int RequiredLevel { get; init; } = 1;

    /// <summary>
    /// Validates that the item does not violate pay-to-win rules:
    /// Supply items must not have a Laun price.
    /// </summary>
    public bool IsPayToWinCompliant =>
        Category != StoreItemCategory.Supply || LaunPrice == 0;
}
