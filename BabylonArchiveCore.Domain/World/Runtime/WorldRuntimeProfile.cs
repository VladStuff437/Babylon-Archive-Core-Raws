namespace BabylonArchiveCore.Domain.World.Runtime;

public sealed class WorldRuntimeProfile
{
    public WorldAccessMode AccessMode { get; init; }
    public bool UnlockAllRooms { get; init; }
    public bool UnlockAllTerminals { get; init; }
    public bool UnlockHardArchivePreview { get; init; }
    public bool EnablePurchaseSimulation { get; init; }
    public bool EnableMissionPageDebugAccess { get; init; }

    public static WorldRuntimeProfile PlayerDefault => new()
    {
        AccessMode = WorldAccessMode.Player,
        UnlockAllRooms = false,
        UnlockAllTerminals = false,
        UnlockHardArchivePreview = false,
        EnablePurchaseSimulation = false,
        EnableMissionPageDebugAccess = false,
    };

    public static WorldRuntimeProfile AdminDefault => new()
    {
        AccessMode = WorldAccessMode.Admin,
        UnlockAllRooms = true,
        UnlockAllTerminals = true,
        UnlockHardArchivePreview = true,
        EnablePurchaseSimulation = true,
        EnableMissionPageDebugAccess = true,
    };
}
