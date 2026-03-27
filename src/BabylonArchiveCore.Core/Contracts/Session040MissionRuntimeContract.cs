namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S040: состояние выполнения миссии в рантайме.
/// </summary>
public sealed class Session040MissionRuntimeContract
{
    public int ContractVersion { get; init; } = 40;

    public required string MissionId { get; init; }

    public required string CurrentNodeId { get; init; }

    public bool IsCompleted { get; init; }

    public int StepCount { get; init; }

    public int MaxStepCount { get; init; } = 128;
}
