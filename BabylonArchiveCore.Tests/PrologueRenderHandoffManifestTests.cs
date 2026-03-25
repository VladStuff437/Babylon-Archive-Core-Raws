using BabylonArchiveCore.Runtime.Scene;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Tests;

public sealed class PrologueRenderHandoffManifestTests
{
    [Fact]
    public void BuildOptionA_UsesDesktopHarnessToWebBabylonStrategy()
    {
        var manifest = PrologueRenderHandoff.BuildOptionA();

        Assert.Equal(PrologueRenderHandoff.SchemaVersion, manifest.SchemaVersion);
        Assert.Equal(PrologueRenderHandoff.StrategyOptionA, manifest.Strategy);
        Assert.Equal("desktop-contour-harness", manifest.SourceRenderer);
        Assert.Equal("web-babylon-mesh-pass", manifest.TargetRenderer);
        Assert.True(manifest.AdapterConfig.UseDesktopContourHarness);
        Assert.True(manifest.AdapterConfig.EnableWebBabylonMeshPass);
        Assert.True(manifest.AdapterConfig.EmitHandoffTelemetry);
    }

    [Fact]
    public void BuildOptionA_EntriesHaveUniqueLogicalIds()
    {
        var manifest = PrologueRenderHandoff.BuildOptionA();

        Assert.Equal(
            manifest.Entries.Count,
            manifest.Entries.Select(e => e.LogicalId).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void BuildOptionA_ArchiveChainOrderMatchesCanonicalSequence()
    {
        var manifest = PrologueRenderHandoff.BuildOptionA();

        var chainOrder = manifest.Entries
            .Where(e => e.NodeKind == PrologueRenderNodeKind.ArchiveChainNode)
            .Select(e => e.LogicalId)
            .ToArray();

        Assert.Equal(ArchiveEntryChain.OrderedNodes, chainOrder);
    }

    [Fact]
    public void BuildOptionA_ModelIdsResolveWithoutFallback()
    {
        var manifest = PrologueRenderHandoff.BuildOptionA();

        Assert.All(
            manifest.Entries,
            entry =>
            {
                var descriptor = ContourRenderRegistry.ResolveDescriptor(entry.ModelId, new Vec3(1f, 1f, 1f));
                Assert.False(descriptor.UsedFallback);
            });
    }

    [Fact]
    public void TryResolveArchiveChainModelId_ReturnsModelForEachNode()
    {
        foreach (var nodeId in ArchiveEntryChain.OrderedNodes)
        {
            var resolved = PrologueRenderHandoff.TryResolveArchiveChainModelId(nodeId, out var modelId);

            Assert.True(resolved);
            Assert.False(string.IsNullOrWhiteSpace(modelId));
        }
    }
}
