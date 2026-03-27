namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S030: расширенное состояние state machine врага.
/// </summary>
public sealed class Session030EnemyStateMachineContract
{
    public required string AgentId { get; init; }

    public required string CurrentState { get; init; }

    public float Aggression { get; init; }

    public float LeashDistance { get; init; }

    public int LastSeenTargetTicks { get; init; }

    public string? PrimaryTargetId { get; init; }
}
