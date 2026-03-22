using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Economy;

namespace BabylonArchiveCore.Runtime.Economy;

/// <summary>
/// Runtime for wallet operations: earning and spending Credits/Launs.
/// Publishes CurrencyEarnedEvent on earnings.
/// </summary>
public sealed class WalletRuntime
{
    private readonly Wallet _wallet;
    private readonly EventBus _eventBus;
    private readonly ILogger _logger;

    public WalletRuntime(Wallet wallet, EventBus eventBus, ILogger logger)
    {
        _wallet = wallet;
        _eventBus = eventBus;
        _logger = logger;
    }

    public Wallet Wallet => _wallet;

    /// <summary>
    /// Awards currency from a gameplay source (mission reward, discovery, etc.).
    /// </summary>
    public void Earn(CurrencyType currency, int amount, string source)
    {
        if (amount <= 0) return;
        _wallet.Earn(currency, amount);
        _logger.Info($"Earned {amount} {currency} from '{source}'. Balance: {_wallet.GetBalance(currency)}");

        _eventBus.Publish(new CurrencyEarnedEvent
        {
            Currency = currency,
            Amount = amount,
            NewBalance = _wallet.GetBalance(currency),
            Source = source,
        });
    }

    /// <summary>
    /// Attempts to spend currency. Returns true on success.
    /// </summary>
    public bool TrySpend(CurrencyType currency, int amount, string reason)
    {
        if (_wallet.TrySpend(currency, amount))
        {
            _logger.Info($"Spent {amount} {currency} for '{reason}'. Balance: {_wallet.GetBalance(currency)}");
            return true;
        }
        _logger.Warn($"Cannot spend {amount} {currency} for '{reason}': insufficient balance ({_wallet.GetBalance(currency)})");
        return false;
    }
}
