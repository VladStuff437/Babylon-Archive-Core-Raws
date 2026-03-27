using BabylonArchiveCore.Core.Missions;

namespace BabylonArchiveCore.Runtime.Missions.Validation;

/// <summary>
/// S043: detects unsafe cycles that have no exits.
/// </summary>
public sealed class CycleSafetyValidator : IMissionValidator
{
    public MissionValidationResult Validate(MissionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var issues = new List<MissionValidationIssue>();
        var byId = definition.Nodes.ToDictionary(n => n.NodeId, StringComparer.Ordinal);
        var reported = new HashSet<string>(StringComparer.Ordinal);
        var stack = new Stack<string>();
        var visiting = new HashSet<string>(StringComparer.Ordinal);
        var visited = new HashSet<string>(StringComparer.Ordinal);

        foreach (var node in definition.Nodes.OrderBy(n => n.NodeId, StringComparer.Ordinal))
        {
            Visit(node.NodeId);
        }

        return new MissionValidationResult { Issues = issues };

        void Visit(string nodeId)
        {
            if (visited.Contains(nodeId))
            {
                return;
            }

            if (!byId.TryGetValue(nodeId, out var node))
            {
                return;
            }

            visiting.Add(nodeId);
            stack.Push(nodeId);

            foreach (var transition in node.Transitions)
            {
                var targetId = transition.TargetNodeId;
                if (!byId.ContainsKey(targetId))
                {
                    continue;
                }

                if (!visited.Contains(targetId))
                {
                    if (visiting.Contains(targetId))
                    {
                        var cycle = new List<string>();
                        foreach (var id in stack)
                        {
                            cycle.Add(id);
                            if (string.Equals(id, targetId, StringComparison.Ordinal))
                            {
                                break;
                            }
                        }

                        cycle.Reverse();

                        if (IsUnsafeCycle(cycle, byId))
                        {
                            var key = string.Join("->", cycle.OrderBy(id => id, StringComparer.Ordinal));
                            if (reported.Add(key))
                            {
                                issues.Add(new MissionValidationIssue
                                {
                                    Code = "MVAL-043-UNSAFE-CYCLE",
                                    NodeId = cycle[0],
                                    Message = $"Cycle without exit detected: {string.Join(" -> ", cycle)}"
                                });
                            }
                        }
                    }
                    else
                    {
                        Visit(targetId);
                    }
                }
            }

            stack.Pop();
            visiting.Remove(nodeId);
            visited.Add(nodeId);
        }
    }

    private static bool IsUnsafeCycle(IReadOnlyCollection<string> cycle, IReadOnlyDictionary<string, MissionNode> byId)
    {
        var cycleSet = new HashSet<string>(cycle, StringComparer.Ordinal);

        foreach (var nodeId in cycle)
        {
            if (!byId.TryGetValue(nodeId, out var node))
            {
                continue;
            }

            if (node.IsTerminal)
            {
                return false;
            }

            var hasExit = node.Transitions.Any(t => !cycleSet.Contains(t.TargetNodeId));
            if (hasExit)
            {
                return false;
            }
        }

        return cycleSet.Count > 0;
    }
}
