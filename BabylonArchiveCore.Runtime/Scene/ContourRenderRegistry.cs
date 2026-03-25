using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Scene;

public enum ContourRenderState
{
    Active,
    Focused,
    Locked,
    Used,
}

public enum ContourFallbackReason
{
    None,
    ModelIdNull,
    SizeUnmapped,
}

public readonly record struct ContourDescriptor(Vec3 Size, bool UsedFallback, ContourFallbackReason FallbackReason)
{
    public string FallbackReasonKey => ContourRenderRegistry.ToTelemetryReasonKey(FallbackReason);
}

public static class ContourRenderRegistry
{
    public static readonly IReadOnlyDictionary<string, Vec3> SizeByModelId = new Dictionary<string, Vec3>(StringComparer.Ordinal)
    {
        [ContourModelIds.CapsuleExit] = new Vec3(1.4f, 2.2f, 1.4f),
        [ContourModelIds.BioScanner] = new Vec3(1.0f, 1.9f, 1.0f),
        [ContourModelIds.SupplyTerminal] = new Vec3(1.2f, 1.95f, 1.0f),
        [ContourModelIds.DroneDock] = new Vec3(1.4f, 1.4f, 1.4f),
        [ContourModelIds.CoreConsole] = new Vec3(1.6f, 2.2f, 1.6f),
        [ContourModelIds.MissionTerminal] = new Vec3(1.3f, 2.0f, 1.0f),
        [ContourModelIds.ResearchTerminal] = new Vec3(1.3f, 2.0f, 1.0f),
        [ContourModelIds.GalleryOverlook] = new Vec3(1.8f, 1.6f, 1.0f),
        [ContourModelIds.ArchiveGate] = new Vec3(2.2f, 2.8f, 1.4f),
        [ContourModelIds.MissionBoard] = new Vec3(1.6f, 2.1f, 1.0f),
        [ContourModelIds.ResearchLab] = new Vec3(1.4f, 2.0f, 1.2f),
        [ContourModelIds.ToolBench] = new Vec3(1.4f, 1.5f, 1.1f),
        [ContourModelIds.ArchiveControl] = new Vec3(1.2f, 2.4f, 1.2f),
        [ContourModelIds.CommerceDesk] = new Vec3(1.2f, 1.8f, 1.0f),
        [ContourModelIds.CommerceHall] = new Vec3(2.2f, 2.1f, 2.2f),
        [ContourModelIds.TechHall] = new Vec3(2.2f, 2.1f, 2.2f),
        [ContourModelIds.ArchivePreview] = new Vec3(1.2f, 1.8f, 1.2f),
        [ContourModelIds.ArchiveCorridor] = new Vec3(2.8f, 2.4f, 2.2f),
        [ContourModelIds.EntryOctagon] = new Vec3(3.4f, 2.5f, 3.4f),
        [ContourModelIds.IndexVestibule] = new Vec3(3.2f, 2.2f, 3.0f),
        [ContourModelIds.ResearchRoom01] = new Vec3(2.8f, 2.1f, 2.2f),
        [ContourModelIds.StackRingPreview] = new Vec3(3.8f, 1.2f, 3.8f),
    };

    public static ContourDescriptor ResolveDescriptor(string? modelId, Vec3 fallbackSize)
    {
        if (string.IsNullOrWhiteSpace(modelId))
            return new ContourDescriptor(fallbackSize, true, ContourFallbackReason.ModelIdNull);

        if (SizeByModelId.TryGetValue(modelId, out var mapped))
            return new ContourDescriptor(mapped, false, ContourFallbackReason.None);

        return new ContourDescriptor(fallbackSize, true, ContourFallbackReason.SizeUnmapped);
    }

    public static string ToTelemetryReasonKey(ContourFallbackReason reason)
    {
        return reason switch
        {
            ContourFallbackReason.None => "none",
            ContourFallbackReason.ModelIdNull => "modelId-null",
            ContourFallbackReason.SizeUnmapped => "size-unmapped",
            _ => "unknown",
        };
    }

    public static Vec3 GetInteractableFallbackSize(InteractiveType type)
    {
        return type switch
        {
            InteractiveType.Terminal => new Vec3(1.0f, 1.9f, 1.0f),
            InteractiveType.Npc => new Vec3(0.75f, 1.35f, 0.75f),
            InteractiveType.Gate => new Vec3(1.8f, 2.6f, 1.2f),
            _ => new Vec3(0.8f, 1.6f, 0.8f),
        };
    }

    public static Vec3 GetExtraNodeFallbackSize(string nodeId, string? modelId)
    {
        if (!string.IsNullOrWhiteSpace(modelId) && SizeByModelId.TryGetValue(modelId, out var mapped))
            return mapped;

        return nodeId switch
        {
            "archive_preview" => new Vec3(1.2f, 1.8f, 1.2f),
            "mission_board" => new Vec3(1.6f, 2.1f, 1.0f),
            "research_lab" => new Vec3(1.4f, 2.0f, 1.2f),
            "archive_control" => new Vec3(1.2f, 2.4f, 1.2f),
            "tool_bench" => new Vec3(1.4f, 1.5f, 1.1f),
            "commerce_desk" => new Vec3(1.2f, 1.8f, 1.0f),
            "commerce_hall" => new Vec3(2.2f, 2.1f, 2.2f),
            "tech_hall" => new Vec3(2.2f, 2.1f, 2.2f),
            "archive_corridor" => new Vec3(2.8f, 2.4f, 2.2f),
            "entry_octagon" => new Vec3(3.4f, 2.5f, 3.4f),
            "index_vestibule" => new Vec3(3.2f, 2.2f, 3.0f),
            "research_room_01" => new Vec3(2.8f, 2.1f, 2.2f),
            "stack_ring_preview" => new Vec3(3.8f, 1.2f, 3.8f),
            _ => new Vec3(0.9f, 1.7f, 0.9f),
        };
    }

    public static ContourRenderState ResolveRenderState(bool isActive, bool isFocused, bool isVisited)
    {
        if (isFocused)
            return ContourRenderState.Focused;

        if (isVisited)
            return ContourRenderState.Used;

        if (!isActive)
            return ContourRenderState.Locked;

        return ContourRenderState.Active;
    }
}
