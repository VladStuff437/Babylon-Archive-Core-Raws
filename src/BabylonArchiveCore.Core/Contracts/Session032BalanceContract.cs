namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S032: базовый баланс v1.
/// </summary>
public sealed class Session032BalanceContract
{
    public required string BalanceProfileId { get; init; }

    public float PlayerDamageMultiplier { get; init; }

    public float EnemyDamageMultiplier { get; init; }

    public float LootQualityBias { get; init; }

    public float EconomyPriceMultiplier { get; init; }
}
