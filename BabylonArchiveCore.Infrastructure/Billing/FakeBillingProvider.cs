using BabylonArchiveCore.Core.Billing;
using BabylonArchiveCore.Domain.Economy;

namespace BabylonArchiveCore.Infrastructure.Billing;

/// <summary>
/// Fake billing provider for prototype/testing.
/// All transactions succeed instantly against local wallet state.
/// </summary>
public sealed class FakeBillingProvider : IBillingProvider
{
    private readonly Wallet _wallet;

    public FakeBillingProvider(Wallet wallet) => _wallet = wallet;

    public string ProviderName => "FakeBilling";

    public Task<bool> ChargeLaunsAsync(string operatorId, int launAmount, string itemId)
    {
        var success = _wallet.TrySpend(CurrencyType.Launs, launAmount);
        return Task.FromResult(success);
    }

    public Task<int> GetLaunsBalanceAsync(string operatorId)
    {
        return Task.FromResult(_wallet.Launs);
    }
}
