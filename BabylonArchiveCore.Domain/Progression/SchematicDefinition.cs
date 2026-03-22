namespace BabylonArchiveCore.Domain.Progression;

/// <summary>
/// A schematic definition: collecting all fragments and meeting stat requirements
/// unlocks a new capability — not a numeric bonus.
/// </summary>
public sealed class SchematicDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    /// <summary>Number of unique fragments required to complete.</summary>
    public required int RequiredFragments { get; init; }

    /// <summary>Stat requirements to research/activate the schematic.</summary>
    public Dictionary<StatType, int> RequiredStats { get; init; } = new();

    /// <summary>Capability granted upon completion (e.g., "archive_deep_read", "temporal_lens").</summary>
    public required string GrantsCapability { get; init; }

    /// <summary>Other schematics that must be completed first.</summary>
    public List<string> PrerequisiteSchematics { get; init; } = new();
}
