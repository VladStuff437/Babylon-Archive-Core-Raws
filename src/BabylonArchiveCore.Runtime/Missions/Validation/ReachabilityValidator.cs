using BabylonArchiveCore.Core.Missions;

namespace BabylonArchiveCore.Runtime.Missions.Validation;

/// <summary>
/// S041: validates that every mission node is reachable from the start node.
/// </summary>
public sealed class ReachabilityValidator : IMissionValidator
{
    public MissionValidationResult Validate(MissionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var issues = new List<MissionValidationIssue>();
        var start = definition.FindNode(definition.StartNodeId);
        if (start is null)
        {
            issues.Add(new MissionValidationIssue
            {
                Code = "MVAL-041-START-MISSING",
                Message = $"Start node '{definition.StartNodeId}' is missing."
            });

            return new MissionValidationResult { Issues = issues };
        }

        var knownNodeIds = new HashSet<string>(definition.Nodes.Select(n => n.NodeId), StringComparer.Ordinal);
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>();
        queue.Enqueue(start.NodeId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!visited.Add(current))
            {
                continue;
            }

            var node = definition.FindNode(current);
            if (node is null)
            {
                continue;
            }

            foreach (var transition in node.Transitions)
            {
                if (knownNodeIds.Contains(transition.TargetNodeId))
                {
                    queue.Enqueue(transition.TargetNodeId);
                }
            }
        }

        foreach (var nodeId in knownNodeIds.Where(id => !visited.Contains(id)).OrderBy(id => id, StringComparer.Ordinal))
        {
            issues.Add(new MissionValidationIssue
            {
                Code = "MVAL-041-UNREACHABLE",
                NodeId = nodeId,
                Message = $"Node '{nodeId}' is unreachable from start node '{definition.StartNodeId}'."
            });
        }

        return new MissionValidationResult { Issues = issues };
    }
}
