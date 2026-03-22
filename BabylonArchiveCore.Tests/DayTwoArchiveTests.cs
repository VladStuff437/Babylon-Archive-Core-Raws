using BabylonArchiveCore.Domain.Archive;
using BabylonArchiveCore.Domain.World;
using BabylonArchiveCore.Runtime.Archive;

namespace BabylonArchiveCore.Tests;

public class DayTwoArchiveTests
{
    // --- ArchiveAddress ---

    [Fact]
    public void ArchiveAddress_SameSeedProducesSameResult()
    {
        var addr = new ArchiveAddress(0, 3, 1, 2, 0, 15, 3);
        var seed1 = addr.ToSeed(42);
        var seed2 = addr.ToSeed(42);

        Assert.Equal(seed1, seed2);
    }

    [Fact]
    public void ArchiveAddress_DifferentAddressesProduceDifferentSeeds()
    {
        var addr1 = new ArchiveAddress(0, 0, 0, 0, 0, 0, 0);
        var addr2 = new ArchiveAddress(0, 0, 0, 0, 0, 0, 1);

        Assert.NotEqual(addr1.ToSeed(42), addr2.ToSeed(42));
    }

    [Fact]
    public void ArchiveAddress_DifferentWorldSeedsProduceDifferentResults()
    {
        var addr = new ArchiveAddress(1, 2, 3, 4, 5, 6, 7);

        Assert.NotEqual(addr.ToSeed(1), addr.ToSeed(2));
    }

    [Fact]
    public void ArchiveAddress_ParseRoundtrip()
    {
        var original = new ArchiveAddress(1, 3, 2, 5, 0, 15, 42);
        var canonical = original.ToCanonical();
        var parsed = ArchiveAddress.Parse(canonical);

        Assert.Equal(original, parsed);
    }

    [Fact]
    public void ArchiveAddress_CanonicalFormat()
    {
        var addr = new ArchiveAddress(1, 3, 2, 5, 0, 15, 42);
        Assert.Equal("S01.H03.M02.SH05.C00.T015.P042", addr.ToCanonical());
    }

    // --- HardArchiveGenerator ---

    [Fact]
    public void Generator_SameSeedProducesIdenticalGraph()
    {
        var gen = new HardArchiveGenerator();
        var graph1 = gen.Generate(seed: 12345);
        var graph2 = gen.Generate(seed: 12345);

        Assert.Equal(graph1.Nodes.Count, graph2.Nodes.Count);
        foreach (var (id, node1) in graph1.Nodes)
        {
            var node2 = graph2.Nodes[id];
            Assert.Equal(node1.Tier, node2.Tier);
            Assert.Equal(node1.HexQ, node2.HexQ);
            Assert.Equal(node1.HexR, node2.HexR);
            Assert.Equal(node1.NodeType, node2.NodeType);
            Assert.Equal(node1.ExitUp, node2.ExitUp);
            Assert.Equal(node1.ExitDown, node2.ExitDown);
            Assert.Equal(node1.Exits.OrderBy(e => e.Key).ToList(),
                         node2.Exits.OrderBy(e => e.Key).ToList());
        }
    }

    [Fact]
    public void Generator_DifferentSeedsProduceDifferentGraphs()
    {
        var gen = new HardArchiveGenerator();
        var g1 = gen.Generate(seed: 111);
        var g2 = gen.Generate(seed: 222);

        // Node types or positions should differ for different seeds
        var types1 = g1.Nodes.Values.Select(n => n.NodeType).OrderBy(t => t).ToList();
        var coords1 = g1.Nodes.Values.Select(n => (n.HexQ, n.HexR)).OrderBy(c => c).ToList();
        var types2 = g2.Nodes.Values.Select(n => n.NodeType).OrderBy(t => t).ToList();
        var coords2 = g2.Nodes.Values.Select(n => (n.HexQ, n.HexR)).OrderBy(c => c).ToList();

        Assert.True(
            !types1.SequenceEqual(types2) || !coords1.SequenceEqual(coords2),
            "Different seeds should produce structurally different graphs");
    }

    [Fact]
    public void Generator_ProducesConnectedGraph_TwoTiers()
    {
        var gen = new HardArchiveGenerator();
        var graph = gen.Generate(seed: 99999, tiers: 2, nodesPerTier: 8);

        Assert.True(ReachabilityValidator.IsValid(graph));
    }

    [Fact]
    public void Generator_ProducesConnectedGraph_ThreeTiers()
    {
        var gen = new HardArchiveGenerator();
        var graph = gen.Generate(seed: 77777, tiers: 3, nodesPerTier: 10);

        Assert.True(ReachabilityValidator.IsValid(graph));
        Assert.True(graph.Nodes.Count >= 3); // at least 1 per tier
    }

    [Fact]
    public void Generator_AllExitsBidirectional()
    {
        var gen = new HardArchiveGenerator();
        var graph = gen.Generate(seed: 55555, tiers: 2, nodesPerTier: 8);

        foreach (var node in graph.Nodes.Values)
        {
            foreach (var (dir, neighborId) in node.Exits)
            {
                var neighbor = graph.Nodes[neighborId];
                var opposite = HardArchiveGenerator.Opposite(dir);
                Assert.True(neighbor.Exits.ContainsKey(opposite),
                    $"Node {node.Id} → {dir} → Node {neighborId}: reverse exit missing");
                Assert.Equal(node.Id, neighbor.Exits[opposite]);
            }

            if (node.ExitUp is { } upId)
                Assert.Equal(node.Id, graph.Nodes[upId].ExitDown);

            if (node.ExitDown is { } downId)
                Assert.Equal(node.Id, graph.Nodes[downId].ExitUp);
        }
    }

    // --- ReachabilityValidator ---

    [Fact]
    public void Validator_DetectsDisconnectedNode()
    {
        var graph = new ArchiveGraph
        {
            Seed = 0,
            EntryNodeId = 0,
            TierCount = 1,
            Nodes = new Dictionary<int, ArchiveNode>
            {
                [0] = new() { Id = 0, Tier = 0, HexQ = 0, HexR = 0, NodeType = ArchiveNodeType.Crossroads },
                [1] = new() { Id = 1, Tier = 0, HexQ = 1, HexR = 0, NodeType = ArchiveNodeType.Corridor },
            },
        };

        Assert.False(ReachabilityValidator.AllNodesReachable(graph));
    }

    [Fact]
    public void Validator_DetectsDeadEnd()
    {
        var isolated = new ArchiveNode
        {
            Id = 0, Tier = 0, HexQ = 0, HexR = 0, NodeType = ArchiveNodeType.ShelfRoom,
        };

        var graph = new ArchiveGraph
        {
            Seed = 0,
            EntryNodeId = 0,
            TierCount = 1,
            Nodes = new Dictionary<int, ArchiveNode> { [0] = isolated },
        };

        Assert.False(ReachabilityValidator.NoDeadEnds(graph));
    }

    // --- WorldState ---

    [Fact]
    public void WorldState_AxesClampWithinBounds()
    {
        var state = new WorldState { WorldSeed = 42 };

        state.AdjustMoral(150);
        Assert.Equal(100, state.MoralAxis);

        state.AdjustMoral(-300);
        Assert.Equal(-100, state.MoralAxis);

        state.AdjustTechnoArcane(200);
        Assert.Equal(100, state.TechnoArcaneAxis);
    }

    [Fact]
    public void WorldState_ConsequenceFlagsWork()
    {
        var state = new WorldState { WorldSeed = 42 };

        Assert.False(state.HasFlag("first_contact"));
        state.SetFlag("first_contact");
        Assert.True(state.HasFlag("first_contact"));
        state.SetFlag("first_contact", false);
        Assert.False(state.HasFlag("first_contact"));
    }

    [Fact]
    public void WorldState_EntityRelationsClamp()
    {
        var state = new WorldState { WorldSeed = 42 };

        state.AdjustRelation("CORE", 80);
        Assert.Equal(80, state.EntityRelations["CORE"]);

        state.AdjustRelation("CORE", 50);
        Assert.Equal(100, state.EntityRelations["CORE"]);

        state.AdjustRelation("CORE", -250);
        Assert.Equal(-100, state.EntityRelations["CORE"]);
    }
}
