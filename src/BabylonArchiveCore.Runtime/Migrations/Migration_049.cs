using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S049: room archetypes contract.
/// </summary>
public sealed class Migration_049
{
    public Session049RoomArchetypesContract Migrate(object? legacyState)
    {
        if (legacyState is Session049RoomArchetypesContract existing)
        {
            return existing;
        }

        return new Session049RoomArchetypesContract
        {
            ContractVersion = 49,
            CatalogVersion = "v1",
            ArchetypeIds = new[] { "intro-combat-hall", "archive-puzzle-node", "recovery-sanctum" },
            HasFallbackArchetype = true
        };
    }
}
