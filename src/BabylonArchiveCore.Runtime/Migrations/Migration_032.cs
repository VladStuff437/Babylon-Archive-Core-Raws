using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S032: базовый баланс v1.
/// </summary>
public sealed class Migration_032
{
    public Session032BalanceContract Migrate(object? legacyState)
    {
        if (legacyState is Session032BalanceContract existing)
        {
            return existing;
        }

        return new Session032BalanceContract
        {
            BalanceProfileId = "baseline-v1",
            PlayerDamageMultiplier = 1f,
            EnemyDamageMultiplier = 1f,
            LootQualityBias = 0f,
            EconomyPriceMultiplier = 1f
        };
    }
}
