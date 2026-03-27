using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S027: контур боевого ввода.
/// </summary>
public sealed class Migration_027
{
    public Session027CombatInputContract Migrate(object? legacyState)
    {
        if (legacyState is Session027CombatInputContract existing)
        {
            return existing;
        }

        return new Session027CombatInputContract
        {
            ActorId = "player-1",
            TargetQueue = Array.Empty<string>(),
            CurrentTargetId = null,
            IsTargetingEnabled = true,
            IsMissionTransitionLocked = false
        };
    }
}
