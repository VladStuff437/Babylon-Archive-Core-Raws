using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S029: состояние перцепции ИИ.
/// </summary>
public sealed class Migration_029
{
    public Session029PerceptionContract Migrate(object? legacyState)
    {
        if (legacyState is Session029PerceptionContract existing)
        {
            return existing;
        }

        return new Session029PerceptionContract
        {
            AgentId = "enemy-1",
            DetectionRadius = 10f,
            AlertThreshold = 0.4f,
            VisibleTargets = Array.Empty<string>(),
            PrimaryTargetId = null
        };
    }
}
