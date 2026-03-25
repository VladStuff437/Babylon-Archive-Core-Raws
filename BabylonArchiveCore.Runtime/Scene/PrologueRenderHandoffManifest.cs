using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Scene;

public enum PrologueRenderNodeKind
{
    Interactable,
    ExtraNode,
    ArchiveChainNode,
}

public sealed record PrologueRenderAdapterConfig(
    bool UseDesktopContourHarness,
    bool EnableWebBabylonMeshPass,
    bool EmitHandoffTelemetry)
{
    public static readonly PrologueRenderAdapterConfig OptionA = new(
        UseDesktopContourHarness: true,
        EnableWebBabylonMeshPass: true,
        EmitHandoffTelemetry: true);
}

public sealed record PrologueRenderHandoffEntry(
    string LogicalId,
    string ModelId,
    PrologueRenderNodeKind NodeKind,
    Vec3 ContourSize,
    bool SupportsDesktopContour,
    bool SupportsWebBabylonMesh);

public sealed record PrologueRenderHandoffManifest(
    string SchemaVersion,
    string Strategy,
    string SourceRenderer,
    string TargetRenderer,
    PrologueRenderAdapterConfig AdapterConfig,
    IReadOnlyList<PrologueRenderHandoffEntry> Entries);

public static class PrologueRenderHandoff
{
    public const string SchemaVersion = "1.0.0";
    public const string StrategyOptionA = "desktop-contour-harness-to-web-babylon-mesh";

    private static readonly IReadOnlyDictionary<string, string> ArchiveChainModelByNodeId = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        [ArchiveEntryChain.ArchiveControl] = ContourModelIds.ArchiveControl,
        [ArchiveEntryChain.ArchiveCorridor] = ContourModelIds.ArchiveCorridor,
        [ArchiveEntryChain.EntryOctagon] = ContourModelIds.EntryOctagon,
        [ArchiveEntryChain.IndexVestibule] = ContourModelIds.IndexVestibule,
        [ArchiveEntryChain.ResearchRoom01] = ContourModelIds.ResearchRoom01,
        [ArchiveEntryChain.StackRingPreview] = ContourModelIds.StackRingPreview,
    };

    private static readonly IReadOnlyList<(string LogicalId, string ModelId, PrologueRenderNodeKind Kind)> OptionAEntries =
    [
        ("capsule_exit", ContourModelIds.CapsuleExit, PrologueRenderNodeKind.Interactable),
        ("bio_scanner", ContourModelIds.BioScanner, PrologueRenderNodeKind.Interactable),
        ("supply_terminal", ContourModelIds.SupplyTerminal, PrologueRenderNodeKind.Interactable),
        ("drone_dock", ContourModelIds.DroneDock, PrologueRenderNodeKind.Interactable),
        ("core_console", ContourModelIds.CoreConsole, PrologueRenderNodeKind.Interactable),
        ("op_terminal", ContourModelIds.MissionTerminal, PrologueRenderNodeKind.Interactable),
        ("research_terminal", ContourModelIds.ResearchTerminal, PrologueRenderNodeKind.Interactable),
        ("gallery_overlook", ContourModelIds.GalleryOverlook, PrologueRenderNodeKind.Interactable),
        ("archive_gate", ContourModelIds.ArchiveGate, PrologueRenderNodeKind.Interactable),
        ("mission_board", ContourModelIds.MissionBoard, PrologueRenderNodeKind.ExtraNode),
        ("research_lab", ContourModelIds.ResearchLab, PrologueRenderNodeKind.ExtraNode),
        ("tool_bench", ContourModelIds.ToolBench, PrologueRenderNodeKind.ExtraNode),
        ("commerce_desk", ContourModelIds.CommerceDesk, PrologueRenderNodeKind.ExtraNode),
        ("commerce_hall", ContourModelIds.CommerceHall, PrologueRenderNodeKind.ExtraNode),
        ("tech_hall", ContourModelIds.TechHall, PrologueRenderNodeKind.ExtraNode),
        ("archive_preview", ContourModelIds.ArchivePreview, PrologueRenderNodeKind.ExtraNode),
        (ArchiveEntryChain.ArchiveControl, ContourModelIds.ArchiveControl, PrologueRenderNodeKind.ArchiveChainNode),
        (ArchiveEntryChain.ArchiveCorridor, ContourModelIds.ArchiveCorridor, PrologueRenderNodeKind.ArchiveChainNode),
        (ArchiveEntryChain.EntryOctagon, ContourModelIds.EntryOctagon, PrologueRenderNodeKind.ArchiveChainNode),
        (ArchiveEntryChain.IndexVestibule, ContourModelIds.IndexVestibule, PrologueRenderNodeKind.ArchiveChainNode),
        (ArchiveEntryChain.ResearchRoom01, ContourModelIds.ResearchRoom01, PrologueRenderNodeKind.ArchiveChainNode),
        (ArchiveEntryChain.StackRingPreview, ContourModelIds.StackRingPreview, PrologueRenderNodeKind.ArchiveChainNode),
    ];

    public static PrologueRenderHandoffManifest BuildOptionA()
    {
        var entries = OptionAEntries
            .Select(entry => new PrologueRenderHandoffEntry(
                entry.LogicalId,
                entry.ModelId,
                entry.Kind,
                ResolveContourSize(entry.ModelId),
                SupportsDesktopContour: true,
                SupportsWebBabylonMesh: true))
            .ToArray();

        return new PrologueRenderHandoffManifest(
            SchemaVersion,
            StrategyOptionA,
            SourceRenderer: "desktop-contour-harness",
            TargetRenderer: "web-babylon-mesh-pass",
            AdapterConfig: PrologueRenderAdapterConfig.OptionA,
            Entries: entries);
    }

    public static bool TryResolveArchiveChainModelId(string nodeId, out string modelId)
    {
        return ArchiveChainModelByNodeId.TryGetValue(nodeId, out modelId!);
    }

    private static Vec3 ResolveContourSize(string modelId)
    {
        var descriptor = ContourRenderRegistry.ResolveDescriptor(modelId, new Vec3(1.0f, 1.0f, 1.0f));
        return descriptor.Size;
    }
}
