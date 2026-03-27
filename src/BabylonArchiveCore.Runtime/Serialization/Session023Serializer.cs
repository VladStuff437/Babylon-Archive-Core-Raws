using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор инвентаря S023.
/// </summary>
public sealed class Session023Serializer
{
    public string Serialize(Session023InventoryContract state) =>
        JsonSerializer.Serialize(state);

    public Session023InventoryContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session023InventoryContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
