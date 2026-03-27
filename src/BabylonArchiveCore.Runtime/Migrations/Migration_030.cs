using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S030: состояние state machine врага.
/// </summary>
public sealed class Migration_030
{
    public Session030EnemyStateMachineContract Migrate(object? legacyState)
    {
        if (legacyState is Session030EnemyStateMachineContract existing)
        {
            return existing;
        }

        return new Session030EnemyStateMachineContract
        {
            AgentId = "enemy-1",
            CurrentState = "Idle",
            Aggression = 0.5f,
            LeashDistance = 15f,
            LastSeenTargetTicks = 0,
            PrimaryTargetId = null
        };
    }
}
