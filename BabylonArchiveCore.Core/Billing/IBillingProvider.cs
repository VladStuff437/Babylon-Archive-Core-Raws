using BabylonArchiveCore.Domain.Economy;

namespace BabylonArchiveCore.Core.Billing;

/// <summary>
/// Abstraction for billing/payment providers.
/// In prototype: FakeBillingProvider. In production: mobile payment SDK integration.
/// </summary>
public interface IBillingProvider
{
    /// <summary>
    /// Attempts to charge the player for a Launs purchase.
    /// Returns true if the transaction succeeds.
    /// </summary>
    Task<bool> ChargeLaunsAsync(string operatorId, int launAmount, string itemId);

    /// <summary>
    /// Returns the current Launs balance from the billing backend.
    /// For fake provider, this returns the locally stored balance.
    /// </summary>
    Task<int> GetLaunsBalanceAsync(string operatorId);

    /// <summary>Provider name for logging.</summary>
    string ProviderName { get; }
}
