using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S033: лут-система.
/// </summary>
public sealed class Migration_033
{
    public Session033LootContract Migrate(object? legacyState)
    {
        if (legacyState is Session033LootContract existing)
        {
            return existing;
        }

        return new Session033LootContract
        {
            BalanceProfileId = "session-033",
            LuckBias = 0f,
            RarityWeights = new Dictionary<string, int>(StringComparer.Ordinal)
            {
                ["common"] = 60,
                ["uncommon"] = 25,
                ["rare"] = 10,
                ["epic"] = 4,
                ["legendary"] = 1
            }
        };
    }
}
