using BabylonArchiveCore.Domain.Progression;

namespace BabylonArchiveCore.Domain.Player;

/// <summary>
/// Operator identity for the prologue: name + progression profile.
/// </summary>
public sealed class OperatorIdentity
{
    public string Name { get; init; } = "Алан Арквейн";
    public string NameLatin { get; init; } = "Alan Arcwain";
    public int ClearanceLevel { get; init; } = 0;
    public OperatorProfile Profile { get; init; } = new();

    /// <summary>Award XP on prologue interactions.</summary>
    public static readonly Dictionary<string, int> InteractionXp = new()
    {
        ["capsule_exit"] = 10,
        ["bio_scanner"] = 15,
        ["supply_terminal"] = 20,
        ["drone_dock"] = 25,
        ["core_console"] = 50,
        ["op_terminal"] = 10,
        ["research_terminal"] = 10,
        ["gallery_overlook"] = 15,
        ["archive_gate"] = 5,
    };
}
