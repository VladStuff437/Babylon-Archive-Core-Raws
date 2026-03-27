using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization;

/// <summary>
/// Сериализатор контракта осей мира S036.
/// </summary>
public sealed class Session036Serializer
{
    public string Serialize(Session036WorldAxesContract state) => JsonSerializer.Serialize(state);

    public Session036WorldAxesContract Deserialize(string json) =>
        JsonSerializer.Deserialize<Session036WorldAxesContract>(json) ?? throw new InvalidOperationException("Deserialization failed");
}
