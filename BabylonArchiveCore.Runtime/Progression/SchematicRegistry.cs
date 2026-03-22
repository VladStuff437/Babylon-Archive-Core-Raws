using BabylonArchiveCore.Domain.Progression;

namespace BabylonArchiveCore.Runtime.Progression;

/// <summary>
/// Tracks collected schematic fragments and resolves schematic completion.
/// </summary>
public sealed class SchematicRegistry
{
    private readonly Dictionary<string, SchematicDefinition> _definitions = new();
    private readonly Dictionary<string, HashSet<string>> _collectedFragments = new();
    private readonly HashSet<string> _completedSchematics = new();

    public IReadOnlySet<string> CompletedSchematics => _completedSchematics;

    public void RegisterSchematic(SchematicDefinition def) => _definitions[def.Id] = def;

    /// <summary>
    /// Collects a fragment. Returns true if it was new.
    /// </summary>
    public bool CollectFragment(SchematicFragment fragment)
    {
        if (!_collectedFragments.TryGetValue(fragment.SchematicId, out var frags))
        {
            frags = new HashSet<string>();
            _collectedFragments[fragment.SchematicId] = frags;
        }
        return frags.Add(fragment.Id);
    }

    /// <summary>
    /// Number of collected fragments for a schematic.
    /// </summary>
    public int FragmentCount(string schematicId) =>
        _collectedFragments.TryGetValue(schematicId, out var f) ? f.Count : 0;

    /// <summary>
    /// Attempts to complete a schematic given the operator's profile.
    /// Returns true if newly completed.
    /// </summary>
    public bool TryComplete(string schematicId, OperatorProfile profile)
    {
        if (_completedSchematics.Contains(schematicId)) return false;
        if (!_definitions.TryGetValue(schematicId, out var def)) return false;

        // Check fragment count
        var collected = FragmentCount(schematicId);
        if (collected < def.RequiredFragments) return false;

        // Check stat requirements
        foreach (var req in def.RequiredStats)
        {
            if (profile.GetStat(req.Key) < req.Value) return false;
        }

        // Check prerequisite schematics
        foreach (var prereq in def.PrerequisiteSchematics)
        {
            if (!_completedSchematics.Contains(prereq)) return false;
        }

        _completedSchematics.Add(schematicId);
        return true;
    }

    /// <summary>
    /// Returns all capabilities granted by completed schematics.
    /// </summary>
    public HashSet<string> GetCapabilities()
    {
        var caps = new HashSet<string>();
        foreach (var id in _completedSchematics)
        {
            if (_definitions.TryGetValue(id, out var def) && def.GrantsCapability is not null)
                caps.Add(def.GrantsCapability);
        }
        return caps;
    }
}
