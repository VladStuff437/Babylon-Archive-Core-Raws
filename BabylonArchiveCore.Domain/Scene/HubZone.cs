namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// A spatial zone within Hub A-0, containing interactive objects.
/// Session 2: upgraded to 3D (Vec3 / Bounds3D).
/// </summary>
public sealed class HubZone
{
    public required HubZoneId Id { get; init; }
    public required string Name { get; init; }
    public required Vec3 Position { get; init; }
    public required Bounds3D Bounds { get; init; }
    public List<InteractableObject> Objects { get; init; } = new();
}
