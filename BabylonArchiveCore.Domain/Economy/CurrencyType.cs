namespace BabylonArchiveCore.Domain.Economy;

/// <summary>
/// Currency types in the game economy.
/// Credits: earned in-game through missions, exploration, discoveries.
/// Launs: premium convenience currency (no pay-to-win, only convenience/cosmetics).
/// </summary>
public enum CurrencyType
{
    /// <summary>In-game currency earned through gameplay.</summary>
    Credits,

    /// <summary>Premium convenience currency (never grants exclusive power).</summary>
    Launs,
}
