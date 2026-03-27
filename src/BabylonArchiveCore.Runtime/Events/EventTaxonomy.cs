namespace BabylonArchiveCore.Runtime.Events;

/// <summary>
/// Базовая таксономия runtime-событий для S015.
/// </summary>
public static class EventTaxonomy
{
    public const string DataDomain = "data";
    public const string RuntimeDomain = "runtime";
    public const string InputDomain = "input";

    public const string MigrationCategory = "migration";
    public const string ValidationCategory = "validation";
    public const string LoopCategory = "game-loop";
    public const string StateCategory = "state";
    public const string CommandCategory = "command";
    public const string ActionMapCategory = "action-map";
    public const string ControlProfileCategory = "control-profile";
    public const string SeedCompositionCategory = "seed-composition";
    public const string GeneratorCoreCategory = "generator-core";
    public const string RoomArchetypeCategory = "room-archetype";
}
