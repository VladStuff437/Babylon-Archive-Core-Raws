using BabylonArchiveCore.Domain.Archive;

namespace BabylonArchiveCore.Runtime.Archive;

/// <summary>
/// Validates structural integrity of a generated <see cref="ArchiveGraph"/>:
/// all nodes reachable from the entry, no dead-end nodes without exits.
/// </summary>
public static class ReachabilityValidator
{
    /// <summary>
    /// BFS from the entry node. Returns true if every node in the graph is reachable
    /// via flat hex exits and vertical (Up/Down) tier transitions.
    /// </summary>
    public static bool AllNodesReachable(ArchiveGraph graph)
    {
        if (graph.Nodes.Count == 0) return true;

        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(graph.EntryNodeId);
        visited.Add(graph.EntryNodeId);

        while (queue.Count > 0)
        {
            var node = graph.Nodes[queue.Dequeue()];

            foreach (var neighborId in node.Exits.Values)
            {
                if (visited.Add(neighborId))
                    queue.Enqueue(neighborId);
            }

            if (node.ExitUp is { } upId && visited.Add(upId))
                queue.Enqueue(upId);

            if (node.ExitDown is { } downId && visited.Add(downId))
                queue.Enqueue(downId);
        }

        return visited.Count == graph.Nodes.Count;
    }

    /// <summary>
    /// Returns true if every node has at least one exit (no isolated rooms).
    /// </summary>
    public static bool NoDeadEnds(ArchiveGraph graph) =>
        graph.Nodes.Values.All(n => n.ExitCount > 0);

    /// <summary>
    /// Full validation: all nodes reachable from entry AND no dead ends.
    /// </summary>
    public static bool IsValid(ArchiveGraph graph) =>
        AllNodesReachable(graph) && NoDeadEnds(graph);
}
