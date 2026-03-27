using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S023: инвентарь v1.
/// </summary>
public sealed class Migration_023
{
    public Session023InventoryContract Migrate(object? legacyState)
    {
        if (legacyState is Session023InventoryContract existing)
            return existing;
        return new Session023InventoryContract
        {
            OwnerId = "player-1",
            Capacity = 20,
            Slots = Array.Empty<InventorySlot>()
        };
    }
}
