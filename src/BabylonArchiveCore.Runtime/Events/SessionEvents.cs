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
}
