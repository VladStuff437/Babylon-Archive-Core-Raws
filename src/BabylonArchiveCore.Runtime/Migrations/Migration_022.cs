using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S022: инициализация контракта взаимодействий.
/// </summary>
public sealed class Migration_022
{
    public Session022InteractionContract Migrate(object? legacyState)
    {
        if (legacyState is Session022InteractionContract existing)
            return existing;
        return new Session022InteractionContract
        {
            InteractionType = "Examine",
            TargetId = "default",
            ActorId = "player-1"
        };
    }
}
