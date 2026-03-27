namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S029: перцепция ИИ врагов.
/// </summary>
public sealed class Session029PerceptionContract
{
    public required string AgentId { get; init; }

    public float DetectionRadius { get; init; }

    public float AlertThreshold { get; init; }

    public required string[] VisibleTargets { get; init; }

    public string? PrimaryTargetId { get; init; }
}
