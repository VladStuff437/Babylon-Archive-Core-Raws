namespace BabylonArchiveCore.Domain.Economy;

/// <summary>
/// Result of a purchase attempt.
/// </summary>
public sealed class PurchaseResult
{
    public bool Success { get; init; }
    public string? ErrorReason { get; init; }
    public string? ItemId { get; init; }
    public CurrencyType? CurrencyUsed { get; init; }
    public int AmountCharged { get; init; }

    public static PurchaseResult Ok(string itemId, CurrencyType currency, int amount) => new()
    {
        Success = true,
        ItemId = itemId,
        CurrencyUsed = currency,
        AmountCharged = amount,
    };

    public static PurchaseResult Fail(string reason) => new()
    {
        Success = false,
        ErrorReason = reason,
    };
}
