using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S025: состояние статусной системы.
/// </summary>
public sealed class Migration_025
{
    public Session025StatusSystemContract Migrate(object? legacyState)
    {
        if (legacyState is Session025StatusSystemContract existing)
        {
            return existing;
        }

        return new Session025StatusSystemContract
        {
            OwnerId = "player-1",
            Statuses = Array.Empty<StatusState>()
        };
    }
}
