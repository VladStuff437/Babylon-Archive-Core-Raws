namespace BabylonArchiveCore.Domain.Progression;

/// <summary>
/// A single fragment of a schematic. Multiple fragments are needed to complete a schematic.
/// Fragments are found by exploring archive tomes or completing missions.
/// </summary>
public sealed class SchematicFragment
{
    public required string Id { get; init; }

    /// <summary>Which schematic this fragment belongs to.</summary>
    public required string SchematicId { get; init; }

    /// <summary>Human-readable description (what was found, where).</summary>
    public string? Description { get; init; }
}
