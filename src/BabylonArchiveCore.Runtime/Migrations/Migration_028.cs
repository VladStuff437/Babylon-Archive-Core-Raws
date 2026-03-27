using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S028: тиковая автоатака.
/// </summary>
public sealed class Migration_028
{
    public Session028AutoAttackContract Migrate(object? legacyState)
    {
        if (legacyState is Session028AutoAttackContract existing)
        {
            return existing;
        }

        return new Session028AutoAttackContract
        {
            ActorId = "player-1",
            TargetId = null,
            IsActive = false,
            AttackIntervalTicks = 1,
            IsMissionTransitionLocked = false
        };
    }
}
