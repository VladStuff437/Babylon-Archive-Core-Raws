namespace BabylonArchiveCore.Runtime.AI.StateMachine;

/// <summary>
/// Простая стейт-машина AI с переходами.
/// </summary>
public sealed class AIStateMachine
{
    public AIState Current { get; private set; } = AIState.Idle;

    public void Transition(AIState newState)
    {
        Current = newState;
    }

    public void Update(bool hasTarget, float distanceToTarget, float health)
    {
        if (health <= 0) { Current = AIState.Dead; return; }
        if (health < 20) { Current = AIState.Flee; return; }
        if (!hasTarget) { Current = Current == AIState.Patrol ? AIState.Patrol : AIState.Idle; return; }
        if (distanceToTarget <= 2f) { Current = AIState.Attack; return; }
        Current = AIState.Chase;
    }

    /// <summary>
    /// Обновление стейта из входа перцепции S029.
    /// </summary>
    public void UpdateFromPerception(bool hasLineOfSight, int visibleTargetsCount, float nearestTargetDistance, float health)
    {
        if (health <= 0)
        {
            Current = AIState.Dead;
            return;
        }

        if (health < 20)
        {
            Current = AIState.Flee;
            return;
        }

        if (!hasLineOfSight || visibleTargetsCount <= 0)
        {
            Current = AIState.Patrol;
            return;
        }

        Current = nearestTargetDistance <= 2f ? AIState.Attack : AIState.Chase;
    }

    /// <summary>
    /// Расширённый цикл поведения врага для S030.
    /// </summary>
    public void UpdateEnemyState(EnemyStateInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input.Health <= 0)
        {
            Current = AIState.Dead;
            return;
        }

        if (input.Health < input.FleeHealthThreshold)
        {
            Current = AIState.Flee;
            return;
        }

        if (!input.HasLineOfSight)
        {
            if (input.LastSeenTargetTicks <= input.InvestigationTicks)
            {
                Current = AIState.Investigate;
                return;
            }

            Current = AIState.ReturnToPost;
            return;
        }

        if (input.DistanceToTarget > input.LeashDistance)
        {
            Current = AIState.ReturnToPost;
            return;
        }

        if (input.Aggression < 0.25f)
        {
            Current = AIState.Alert;
            return;
        }

        Current = input.DistanceToTarget <= input.AttackDistance ? AIState.Attack : AIState.Chase;
    }
}

public sealed class EnemyStateInput
{
    public bool HasLineOfSight { get; init; }

    public float DistanceToTarget { get; init; }

    public float LeashDistance { get; init; } = 15f;

    public float Health { get; init; } = 100f;

    public float FleeHealthThreshold { get; init; } = 20f;

    public float Aggression { get; init; } = 0.5f;

    public float AttackDistance { get; init; } = 2f;

    public int LastSeenTargetTicks { get; init; }

    public int InvestigationTicks { get; init; } = 3;
}
