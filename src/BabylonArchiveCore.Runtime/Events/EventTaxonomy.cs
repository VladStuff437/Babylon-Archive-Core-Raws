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
    public const string CameraCategory = "camera";
    public const string InteractionCategory = "interaction";
    public const string InventoryCategory = "inventory";
    public const string CharacterCategory = "character";
    public const string StatusCategory = "status";
    public const string CombatCategory = "combat";
    public const string AICategory = "ai";
    public const string TargetingCategory = "targeting";
    public const string AutoAttackCategory = "auto-attack";
    public const string PerceptionCategory = "perception";
    public const string EnemyStateMachineCategory = "enemy-state-machine";
    public const string FormulaCategory = "formula";
    public const string BalanceCategory = "balance";
    public const string LootCategory = "loot";
    public const string EconomyCategory = "economy";
    public const string MissionCategory = "mission";
    public const string FactionCategory = "faction";
    public const string WorldAxisCategory = "world-axis";
    public const string MissionDefinitionCategory = "mission-definition";
    public const string MissionNodeCategory = "mission-node";
    public const string TransitionEvaluationCategory = "transition-evaluation";
    public const string MissionRuntimeCategory = "mission-runtime";
    public const string GenerationCategory = "generation";
    public const string ReachabilityCategory = "reachability";
    public const string DeadEndCategory = "dead-end";
    public const string CycleSafetyCategory = "cycle-safety";
    public const string FallbackCategory = "fallback";
    public const string MissionPersistenceCategory = "mission-persistence";
    public const string ArchiveAddressCategory = "archive-address";
}
