using System.Text.Json;

namespace BabylonArchiveCore.Runtime.Generation;

public static class RoomArchetypeCatalog
{
    public static IReadOnlyList<RoomArchetypeDefinition> LoadFromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var room = JsonSerializer.Deserialize<RoomArchetypeDefinition>(json);
        if (room is null)
        {
            throw new InvalidOperationException("Room archetype deserialization failed.");
        }

        return new[] { room };
    }

    public static IReadOnlyList<RoomArchetypeDefinition> LoadFromJsonArray(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var rooms = JsonSerializer.Deserialize<RoomArchetypeDefinition[]>(json);
        if (rooms is null || rooms.Length == 0)
        {
            throw new InvalidOperationException("Room archetype array deserialization failed.");
        }

        return rooms;
    }

    public static IReadOnlyList<RoomArchetypeDefinition> SelectForDepth(
        IReadOnlyList<RoomArchetypeDefinition> archetypes,
        int depth)
    {
        ArgumentNullException.ThrowIfNull(archetypes);

        var filtered = archetypes
            .Where(archetype => archetype.SupportsDepth(depth))
            .OrderBy(archetype => archetype.ArchetypeId, StringComparer.Ordinal)
            .ToArray();

        return filtered.Length == 0 ? archetypes : filtered;
    }
}
