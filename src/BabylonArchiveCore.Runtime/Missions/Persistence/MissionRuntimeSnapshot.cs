namespace BabylonArchiveCore.Runtime.Missions.Persistence;

public sealed class MissionRuntimeSnapshot
{
    public required string MissionId { get; init; }

    public int SnapshotVersion { get; init; } = 45;

    public required string CurrentNodeId { get; init; }

    public bool IsCompleted { get; init; }

    public int StepCount { get; init; }

    public required IReadOnlyList<string> VisitedNodeIds { get; init; }

    public string? StateChecksum { get; init; }
}
