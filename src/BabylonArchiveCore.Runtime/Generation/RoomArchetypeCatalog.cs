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
}
