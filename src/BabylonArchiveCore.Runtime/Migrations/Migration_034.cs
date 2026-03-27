using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S034: экономика v1.
/// </summary>
public sealed class Migration_034
{
    public Session034EconomyContract Migrate(object? legacyState)
    {
        if (legacyState is Session034EconomyContract existing)
        {
            return existing;
        }

        return new Session034EconomyContract
        {
            EconomyProfileId = "session-034",
            InflationIndex = 1f,
            BuyMultiplier = 1f,
            SellMultiplier = 1f,
            DefaultFactionModifier = 1f
        };
    }
}
