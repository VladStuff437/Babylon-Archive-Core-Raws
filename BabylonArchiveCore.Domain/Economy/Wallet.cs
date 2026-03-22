namespace BabylonArchiveCore.Domain.Economy;

/// <summary>
/// Player's wallet: tracks Credits and Launs balances.
/// </summary>
public sealed class Wallet
{
    public int Credits { get; set; }
    public int Launs { get; set; }

    public int GetBalance(CurrencyType currency) => currency switch
    {
        CurrencyType.Credits => Credits,
        CurrencyType.Launs => Launs,
        _ => 0,
    };

    public bool CanAfford(CurrencyType currency, int amount) =>
        GetBalance(currency) >= amount;

    public bool TrySpend(CurrencyType currency, int amount)
    {
        if (amount <= 0 || !CanAfford(currency, amount)) return false;
        switch (currency)
        {
            case CurrencyType.Credits: Credits -= amount; break;
            case CurrencyType.Launs: Launs -= amount; break;
        }
        return true;
    }

    public void Earn(CurrencyType currency, int amount)
    {
        if (amount <= 0) return;
        switch (currency)
        {
            case CurrencyType.Credits: Credits += amount; break;
            case CurrencyType.Launs: Launs += amount; break;
        }
    }
}
