using System;
using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.State;

namespace BabylonArchiveCore.Runtime.Migrations
{
    /// <summary>
    /// Миграция состояния камеры для сессии 021.
    /// </summary>
    public sealed class Migration_021
    {
        public Session021CameraContract Migrate(object? legacyState)
        {
            // Пример миграции: если есть старое состояние, преобразовать его в новый контракт
            if (legacyState is Session021CameraContract oldState)
            {
                return oldState;
            }
            // Иначе — создать дефолтное состояние
            return new Session021CameraContract();
        }
    }
}
