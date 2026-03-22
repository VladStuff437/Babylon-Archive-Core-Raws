using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Runtime.Debug;

/// <summary>
/// Allows instant teleportation to any zone's center position.
/// Only functions when <see cref="DebugConfig.AllowTeleport"/> is true.
/// </summary>
public sealed class DebugTeleport
{
    private readonly DebugConfig _config;
    private readonly ILogger _logger;

    public DebugTeleport(DebugConfig config, ILogger logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Teleport the player to the center of the specified zone.
    /// Returns the new position, or null if teleport is disabled or zone not found.
    /// </summary>
    public Vec3? TeleportTo(HubZoneId zoneId, PlayerEntity player, HubA0Runtime hubRuntime)
    {
        if (!_config.Enabled || !_config.AllowTeleport)
        {
            _logger.Warn("DebugTeleport: teleport disabled.");
            return null;
        }

        var zone = hubRuntime.Zones.FirstOrDefault(z => z.Id == zoneId);
        if (zone is null)
        {
            _logger.Warn($"DebugTeleport: zone {zoneId} not found.");
            return null;
        }

        var target = new Vec3(zone.Position.X, 0f, zone.Position.Z);
        player.Position = target;
        _logger.Info($"DebugTeleport: teleported to {zoneId} at ({target.X:F1}, {target.Z:F1})");
        return target;
    }

    /// <summary>
    /// List all available teleport destinations.
    /// </summary>
    public IReadOnlyList<(HubZoneId Id, string Name, Vec3 Position)> ListDestinations(HubA0Runtime hubRuntime)
    {
        return hubRuntime.Zones
            .Select(z => (z.Id, z.Name, z.Position))
            .ToList();
    }
}
