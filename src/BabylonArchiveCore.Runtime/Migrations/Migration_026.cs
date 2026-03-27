using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S026: ядро боевой системы.
/// </summary>
public sealed class Migration_026
{
    public Session026CombatCoreContract Migrate(object? legacyState)
    {
        if (legacyState is Session026CombatCoreContract existing)
        {
            return existing;
        }

        return new Session026CombatCoreContract
        {
            AttackerId = "player-1",
            TargetId = "target-default",
            DirectBaseDamage = 0,
            AoeBaseDamage = 0,
            TargetArmor = 0,
            IsCritical = false
        };
    }
}
