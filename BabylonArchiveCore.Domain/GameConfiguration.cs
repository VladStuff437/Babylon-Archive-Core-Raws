namespace BabylonArchiveCore.Domain;

public sealed class GameConfiguration
{
    public string AppName { get; init; } = "Babylon Archive Core";

    public int SaveVersion { get; init; } = 2;

    public string LogFileName { get; init; } = "bac-runtime.log";

    public int TickDelayMs { get; init; } = 50;
}
