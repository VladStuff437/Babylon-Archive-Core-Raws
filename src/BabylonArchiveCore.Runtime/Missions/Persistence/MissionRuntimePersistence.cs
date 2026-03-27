using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using BabylonArchiveCore.Core.Missions;

namespace BabylonArchiveCore.Runtime.Missions.Persistence;

/// <summary>
/// Save/load helpers for runtime mission state.
/// </summary>
public sealed class MissionRuntimePersistence
{
    public MissionRuntimeSnapshot SaveSnapshot(MissionDefinition definition, MissionRuntimeState state)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(state);

        var visited = state.VisitedNodeIds.OrderBy(id => id, StringComparer.Ordinal).ToArray();
        var checksum = ComputeChecksum(definition.MissionId, state.CurrentNodeId, state.IsCompleted, state.StepCount, visited);

        return new MissionRuntimeSnapshot
        {
            MissionId = definition.MissionId,
            SnapshotVersion = 45,
            CurrentNodeId = state.CurrentNodeId,
            IsCompleted = state.IsCompleted,
            StepCount = state.StepCount,
            VisitedNodeIds = visited,
            StateChecksum = checksum
        };
    }

    public string Serialize(MissionRuntimeSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        return JsonSerializer.Serialize(snapshot);
    }

    public MissionRuntimeSnapshot Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var snapshot = JsonSerializer.Deserialize<MissionRuntimeSnapshot>(json)
            ?? throw new InvalidOperationException("Mission runtime snapshot deserialization failed.");

        if (!string.IsNullOrWhiteSpace(snapshot.StateChecksum))
        {
            var expected = ComputeChecksum(snapshot.MissionId, snapshot.CurrentNodeId, snapshot.IsCompleted, snapshot.StepCount, snapshot.VisitedNodeIds);
            if (!string.Equals(expected, snapshot.StateChecksum, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Mission runtime snapshot checksum validation failed.");
            }
        }

        return snapshot;
    }

    public MissionRuntimeState RestoreState(MissionRuntimeSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        return MissionRuntimeState.Restore(snapshot.CurrentNodeId, snapshot.IsCompleted, snapshot.StepCount, snapshot.VisitedNodeIds);
    }

    private static string ComputeChecksum(string missionId, string currentNodeId, bool isCompleted, int stepCount, IReadOnlyList<string> visitedNodeIds)
    {
        var normalizedVisited = visitedNodeIds.OrderBy(id => id, StringComparer.Ordinal);
        var payload = string.Join('|',
            missionId,
            currentNodeId,
            isCompleted ? "1" : "0",
            stepCount.ToString(),
            string.Join(',', normalizedVisited));

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(bytes);
    }
}
