namespace BabylonArchiveCore.Runtime.Events;

/// <summary>
/// Каталог runtime-событий S011-S013.
/// </summary>
public static class SessionEvents
{
    public const string Session011SchemaLoaded = "session.011.schema.loaded";
    public const string Session012SaveMigrated = "session.012.save.migrated";
    public const string Session013VersionValidated = "session.013.version.validated";
    public const string Session014MigrationPipelineReady = "session.014.migration.pipeline.ready";
    public const string Session015TaxonomyApplied = "session.015.taxonomy.applied";
    public const string Session016GameLoopStarted = "session.016.gameloop.started";
}
