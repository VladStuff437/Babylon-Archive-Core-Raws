using System;
using System.Text.Json;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Serialization
{
    /// <summary>
    /// Сериализатор состояния камеры для сессии 021.
    /// </summary>
    public sealed class Session021Serializer
    {
        public string Serialize(Session021CameraContract state)
        {
            return JsonSerializer.Serialize(state);
        }

        public Session021CameraContract Deserialize(string json)
        {
            return JsonSerializer.Deserialize<Session021CameraContract>(json) ?? new Session021CameraContract();
        }
    }
}
