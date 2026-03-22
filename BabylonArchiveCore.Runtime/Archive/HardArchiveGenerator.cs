using BabylonArchiveCore.Domain.Archive;

namespace BabylonArchiveCore.Runtime.Archive;

/// <summary>
/// Deterministic fractal generator for the Hard-Archive structure.
/// Produces a hex-grid graph with multiple tiers connected by diagonal staircases.
/// Same seed always yields the same graph.
/// </summary>
public sealed class HardArchiveGenerator
{
    /// <summary>
    /// Generates a Hard-Archive graph from the given seed.
    /// </summary>
    /// <param name="seed">Deterministic seed (derived from ArchiveAddress + world seed).</param>
    /// <param name="tiers">Number of vertical layers (ярусы).</param>
    /// <param name="nodesPerTier">Target node count per tier.</param>
    public ArchiveGraph Generate(int seed, int tiers = 2, int nodesPerTier = 8)
    {
        var rng = new Random(seed);
        var nodes = new Dictionary<int, ArchiveNode>();
        var nextId = 0;
        var occupied = new Dictionary<(int tier, int q, int r), int>();
        var tierEntries = new List<int>();

        // --- Build each tier as a connected hex sub-graph ---
        for (var tier = 0; tier < tiers; tier++)
        {
            var entryId = nextId++;
            var entry = new ArchiveNode
            {
                Id = entryId,
                Tier = tier,
                HexQ = 0,
                HexR = 0,
                NodeType = ArchiveNodeType.Crossroads,
            };
            nodes[entryId] = entry;
            occupied[(tier, 0, 0)] = entryId;
            tierEntries.Add(entryId);

            var frontier = new List<int> { entryId };
            var created = 1;

            while (created < nodesPerTier && frontier.Count > 0)
            {
                var srcIdx = rng.Next(frontier.Count);
                var src = nodes[frontier[srcIdx]];

                var available = AvailableDirections(src);
                if (available.Count == 0)
                {
                    frontier.RemoveAt(srcIdx);
                    continue;
                }

                var dir = available[rng.Next(available.Count)];
                var (nq, nr) = Offset(src.HexQ, src.HexR, dir);

                // If the cell is already occupied, add a loop connection instead
                if (occupied.TryGetValue((tier, nq, nr), out var existingId))
                {
                    TryLink(src, dir, nodes[existingId]);
                    continue;
                }

                var node = new ArchiveNode
                {
                    Id = nextId++,
                    Tier = tier,
                    HexQ = nq,
                    HexR = nr,
                    NodeType = PickType(rng, created, nodesPerTier),
                };

                Link(src, dir, node);
                nodes[node.Id] = node;
                occupied[(tier, nq, nr)] = node.Id;
                frontier.Add(node.Id);
                created++;
            }
        }

        // --- Connect adjacent tiers via diagonal staircases ---
        for (var t = 0; t < tiers - 1; t++)
        {
            var upper = nodes.Values
                .Where(n => n.Tier == t && n.ExitDown is null)
                .ToList();
            var lower = nodes.Values
                .Where(n => n.Tier == t + 1 && n.ExitUp is null)
                .ToList();

            if (upper.Count > 0 && lower.Count > 0)
            {
                var u = upper[rng.Next(upper.Count)];
                var d = lower[rng.Next(lower.Count)];
                u.ExitDown = d.Id;
                d.ExitUp = u.Id;
            }
        }

        return new ArchiveGraph
        {
            Seed = seed,
            EntryNodeId = tierEntries[0],
            TierCount = tiers,
            Nodes = nodes,
        };
    }

    // ----- helpers -----

    private static List<HexDirection> AvailableDirections(ArchiveNode node) =>
        Enum.GetValues<HexDirection>()
            .Where(d => !node.Exits.ContainsKey(d))
            .ToList();

    private static void TryLink(ArchiveNode a, HexDirection dir, ArchiveNode b)
    {
        a.Exits.TryAdd(dir, b.Id);
        b.Exits.TryAdd(Opposite(dir), a.Id);
    }

    private static void Link(ArchiveNode a, HexDirection dir, ArchiveNode b)
    {
        a.Exits[dir] = b.Id;
        b.Exits[Opposite(dir)] = a.Id;
    }

    private static ArchiveNodeType PickType(Random rng, int index, int total) =>
        index < total / 3
            ? ArchiveNodeType.Crossroads
            : rng.Next(4) switch
            {
                0 => ArchiveNodeType.Corridor,
                1 => ArchiveNodeType.ShelfRoom,
                2 => ArchiveNodeType.TerminalRoom,
                _ => ArchiveNodeType.Crossroads,
            };

    /// <summary>Axial hex-grid neighbour offset (flat-top orientation).</summary>
    public static (int q, int r) Offset(int q, int r, HexDirection dir) => dir switch
    {
        HexDirection.North     => (q,     r - 1),
        HexDirection.NorthEast => (q + 1, r - 1),
        HexDirection.SouthEast => (q + 1, r),
        HexDirection.South     => (q,     r + 1),
        HexDirection.SouthWest => (q - 1, r + 1),
        HexDirection.NorthWest => (q - 1, r),
        _ => throw new ArgumentOutOfRangeException(nameof(dir)),
    };

    /// <summary>Returns the opposite hex direction.</summary>
    public static HexDirection Opposite(HexDirection dir) => dir switch
    {
        HexDirection.North     => HexDirection.South,
        HexDirection.NorthEast => HexDirection.SouthWest,
        HexDirection.SouthEast => HexDirection.NorthWest,
        HexDirection.South     => HexDirection.North,
        HexDirection.SouthWest => HexDirection.NorthEast,
        HexDirection.NorthWest => HexDirection.SouthEast,
        _ => throw new ArgumentOutOfRangeException(nameof(dir)),
    };
}
