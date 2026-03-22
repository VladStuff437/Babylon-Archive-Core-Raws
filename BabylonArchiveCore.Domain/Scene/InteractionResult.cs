using BabylonArchiveCore.Domain.Player;

namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Result of interacting with an object in the scene.
/// </summary>
public sealed class InteractionResult
{
    public required string ObjectId { get; init; }
    public required bool Success { get; init; }
    public required string Message { get; init; }

    /// <summary>If interaction advanced the rhythm, this is the new phase.</summary>
    public HubRhythmPhase? NewPhase { get; init; }

    /// <summary>Dialogue started by this interaction (if any).</summary>
    public string? DialogueId { get; init; }

    /// <summary>Items granted by this interaction (if any).</summary>
    public IReadOnlyList<InventoryItem>? GrantedItems { get; init; }

    /// <summary>Terminal screen shown (if any).</summary>
    public TerminalScreen? TerminalScreen { get; init; }

    /// <summary>The type of the interacted object.</summary>
    public InteractiveType? ObjectType { get; init; }
}
