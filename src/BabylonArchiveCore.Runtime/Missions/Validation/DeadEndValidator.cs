using BabylonArchiveCore.Core.Missions;

namespace BabylonArchiveCore.Runtime.Missions.Validation;

/// <summary>
/// S042: detects dead-end mission nodes.
/// </summary>
public sealed class DeadEndValidator : IMissionValidator
{
    public MissionValidationResult Validate(MissionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var issues = new List<MissionValidationIssue>();
        foreach (var node in definition.Nodes)
        {
            if (node.IsTerminal)
            {
                continue;
            }

            if (node.Transitions.Count == 0)
            {
                issues.Add(new MissionValidationIssue
                {
                    Code = "MVAL-042-DEADEND",
                    NodeId = node.NodeId,
                    Message = $"Node '{node.NodeId}' is non-terminal and has no outgoing transitions."
                });

                continue;
            }

            if (node.Transitions.All(t => string.Equals(t.TargetNodeId, node.NodeId, StringComparison.Ordinal)))
            {
                issues.Add(new MissionValidationIssue
                {
                    Code = "MVAL-042-SELF-LOOP-DEADEND",
                    NodeId = node.NodeId,
                    Message = $"Node '{node.NodeId}' only points to itself."
                });
            }
        }

        return new MissionValidationResult { Issues = issues };
    }
}
