using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Tests;

public sealed class ContourRenderRegistryTests
{
    private static string? FindContentRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir is not null)
        {
            var contentDir = Path.Combine(dir, "Content");
            if (Directory.Exists(contentDir) && File.Exists(Path.Combine(contentDir, "Zones", "A0_Zones.json")))
                return contentDir;

            dir = Directory.GetParent(dir)?.FullName;
        }

        return null;
    }

    [Fact]
    public void ResolveDescriptor_UsesFallbackForNullModelId()
    {
        var fallback = new Vec3(1f, 2f, 3f);

        var descriptor = ContourRenderRegistry.ResolveDescriptor(null, fallback);

        Assert.True(descriptor.UsedFallback);
        Assert.Equal(ContourFallbackReason.ModelIdNull, descriptor.FallbackReason);
        Assert.Equal("modelId-null", descriptor.FallbackReasonKey);
        Assert.Equal(fallback, descriptor.Size);
    }

    [Fact]
    public void ResolveDescriptor_UsesMappedSizeForKnownModelId()
    {
        var fallback = new Vec3(0.9f, 0.9f, 0.9f);

        var descriptor = ContourRenderRegistry.ResolveDescriptor(ContourModelIds.CoreConsole, fallback);

        Assert.False(descriptor.UsedFallback);
        Assert.Equal(ContourFallbackReason.None, descriptor.FallbackReason);
        Assert.Equal("none", descriptor.FallbackReasonKey);
        Assert.Equal(new Vec3(1.6f, 2.2f, 1.6f), descriptor.Size);
    }

    [Fact]
    public void ResolveDescriptor_UsesFallbackForUnknownModelId()
    {
        var fallback = new Vec3(0.7f, 0.8f, 0.9f);

        var descriptor = ContourRenderRegistry.ResolveDescriptor("ENV_UNKNOWN_CONTOUR", fallback);

        Assert.True(descriptor.UsedFallback);
        Assert.Equal(ContourFallbackReason.SizeUnmapped, descriptor.FallbackReason);
        Assert.Equal("size-unmapped", descriptor.FallbackReasonKey);
        Assert.Equal(fallback, descriptor.Size);
    }

    [Fact]
    public void ToTelemetryReasonKey_MapsKnownReasons()
    {
        Assert.Equal("none", ContourRenderRegistry.ToTelemetryReasonKey(ContourFallbackReason.None));
        Assert.Equal("modelId-null", ContourRenderRegistry.ToTelemetryReasonKey(ContourFallbackReason.ModelIdNull));
        Assert.Equal("size-unmapped", ContourRenderRegistry.ToTelemetryReasonKey(ContourFallbackReason.SizeUnmapped));
    }

    [Theory]
    [InlineData(true, false, false, ContourRenderState.Active)]
    [InlineData(true, true, false, ContourRenderState.Focused)]
    [InlineData(false, false, false, ContourRenderState.Locked)]
    [InlineData(true, false, true, ContourRenderState.Used)]
    [InlineData(false, true, true, ContourRenderState.Focused)]
    public void ResolveRenderState_MapsExpectedState(bool isActive, bool isFocused, bool isVisited, ContourRenderState expected)
    {
        var state = ContourRenderRegistry.ResolveRenderState(isActive, isFocused, isVisited);
        Assert.Equal(expected, state);
    }

    [Fact]
    public void A0Content_ObjectModelIds_AreRegisteredInContourRenderRegistry()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null)
            return;

        var provider = new A0ContentProvider(contentRoot);
        var zones = provider.LoadZones();

        var missing = zones
            .SelectMany(z => z.Objects)
            .Where(o => !string.IsNullOrWhiteSpace(o.ModelId))
            .Where(o => !ContourRenderRegistry.SizeByModelId.ContainsKey(o.ModelId!))
            .Select(o => $"{o.Id}:{o.ModelId}")
            .ToList();

        Assert.Empty(missing);
    }

    [Fact]
    public void Registry_ContainsRequiredExtraNodeModelIds()
    {
        var required = new[]
        {
            ContourModelIds.MissionBoard,
            ContourModelIds.ResearchLab,
            ContourModelIds.ToolBench,
            ContourModelIds.ArchiveControl,
            ContourModelIds.CommerceDesk,
            ContourModelIds.CommerceHall,
            ContourModelIds.TechHall,
            ContourModelIds.ArchivePreview,
            ContourModelIds.ArchiveCorridor,
            ContourModelIds.EntryOctagon,
            ContourModelIds.IndexVestibule,
            ContourModelIds.ResearchRoom01,
            ContourModelIds.StackRingPreview,
        };

        Assert.All(required, modelId => Assert.Contains(modelId, ContourRenderRegistry.SizeByModelId.Keys));
    }
}
