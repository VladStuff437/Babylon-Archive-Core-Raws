namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S049 contract: room archetype catalog and selection guarantees.
/// </summary>
public sealed class Session049RoomArchetypesContract
{
    public int ContractVersion { get; init; } = 49;

    public required string CatalogVersion { get; init; }

    public required string[] ArchetypeIds { get; init; }

    public bool HasFallbackArchetype { get; init; }
}
