namespace BabylonArchiveCore.Runtime.Events;

/// <summary>
/// Каталог runtime-событий S011-S020.
/// </summary>
public static class SessionEvents
{
    public const string Session011SchemaLoaded = "session.011.schema.loaded";
    public const string Session012SaveMigrated = "session.012.save.migrated";
    public const string Session013VersionValidated = "session.013.version.validated";
    public const string Session014MigrationPipelineReady = "session.014.migration.pipeline.ready";
    public const string Session015TaxonomyApplied = "session.015.taxonomy.applied";
    public const string Session016GameLoopStarted = "session.016.gameloop.started";
    public const string Session017RuntimeStateSynchronized = "session.017.runtime.state.synchronized";
    public const string Session018CommandContourValidated = "session.018.command.contour.validated";
    public const string Session019ActionMapResolved = "session.019.action.map.resolved";
    public const string Session020ControlProfilesConsolidated = "session.020.control.profiles.consolidated";
    public const string Session021CameraSystemReady = "session.021.camera.system.ready";
    public const string Session022InteractionContourReady = "session.022.interaction.contour.ready";
    public const string Session023InventoryV1Ready = "session.023.inventory.v1.ready";
    public const string Session024AttributesReady = "session.024.attributes.ready";
    public const string Session025StatusSystemReady = "session.025.status.system.ready";
    public const string Session026CombatCoreReady = "session.026.combat.core.ready";
    public const string Session027CombatInputReady = "session.027.combat.input.ready";
    public const string Session028AutoAttackReady = "session.028.auto.attack.ready";
    public const string Session029PerceptionReady = "session.029.perception.ready";
    public const string Session030EnemyStateMachineReady = "session.030.enemy.state.machine.ready";
    public const string Session031DamageFormulasReady = "session.031.damage.formulas.ready";
    public const string Session032BalanceV1Ready = "session.032.balance.v1.ready";
    public const string Session033LootSystemReady = "session.033.loot.system.ready";
    public const string Session034EconomyV1Ready = "session.034.economy.v1.ready";
    public const string Session035FactionReputationReady = "session.035.faction.reputation.ready";
    public const string Session036WorldAxesReady = "session.036.world.axes.ready";
    public const string Session037MissionDefinitionContractReady = "session.037.mission.definition.contract.ready";
    public const string Session038MissionNodeContractReady = "session.038.mission.node.contract.ready";
    public const string Session039TransitionEvaluationReady = "session.039.transition.evaluation.ready";
    public const string Session040MissionRuntimeReady = "session.040.mission.runtime.ready";
    public const string Session041ReachabilityValidationReady = "session.041.reachability.validation.ready";
    public const string Session042DeadEndValidationReady = "session.042.dead.end.validation.ready";
    public const string Session043CycleSafetyValidationReady = "session.043.cycle.safety.validation.ready";
    public const string Session044FallbackMissionsReady = "session.044.fallback.missions.ready";
    public const string Session045MissionSaveLoadReady = "session.045.mission.save.load.ready";
    public const string Session046ArchiveAddressModelReady = "session.046.archive.address.model.ready";
}
