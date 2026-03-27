namespace BabylonArchiveCore.Core.Missions;

/// <summary>
/// Определение миссии с узлами и стартовой точкой.
/// </summary>
public sealed class MissionDefinition
{
    public int ContractVersion { get; init; } = 37;

    public required string MissionId { get; init; }

    public required string Title { get; init; }

    public required string StartNodeId { get; init; }

    public required IReadOnlyList<MissionNode> Nodes { get; init; }

    public MissionNode? FindNode(string nodeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);
        return Nodes.FirstOrDefault(n => string.Equals(n.NodeId, nodeId, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (Nodes.Count == 0)
        {
            errors.Add("Mission must contain at least one node.");
            return errors;
        }

        var startNode = FindNode(StartNodeId);
        if (startNode is null)
        {
            errors.Add($"Start node '{StartNodeId}' is missing.");
        }
        else if (startNode.IsTerminal)
        {
            errors.Add($"Start node '{StartNodeId}' cannot be terminal.");
        }

        var duplicateNodeIds = Nodes
            .GroupBy(n => n.NodeId, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        foreach (var duplicate in duplicateNodeIds)
        {
            errors.Add($"Duplicate node id '{duplicate}'.");
        }

        var knownNodeIds = Nodes.Select(n => n.NodeId).ToArray();
        foreach (var node in Nodes)
        {
            errors.AddRange(node.ValidateTransitions(knownNodeIds));
        }

        return errors;
    }
}
