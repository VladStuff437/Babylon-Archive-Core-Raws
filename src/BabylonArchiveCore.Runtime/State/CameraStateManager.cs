using System;
using System.Collections.Generic;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.State
{
    /// <summary>
    /// Менеджер состояния камеры для сессии 021.
    /// </summary>
    public sealed class CameraStateManager
    {
        private Session021CameraContract _state;

        public CameraStateManager()
        {
            _state = new Session021CameraContract();
        }

        public Session021CameraContract GetState() => _state;

        public void SetMode(string mode)
        {
            if (mode != "Follow" && mode != "Aim" && mode != "Inspect")
                throw new ArgumentException($"Invalid camera mode: {mode}");
            _state.Mode = mode;
        }

        public void UpdatePosition(float x, float y, float z)
        {
            _state.PositionX = x;
            _state.PositionY = y;
            _state.PositionZ = z;
        }

        public void UpdateTarget(float x, float y, float z)
        {
            _state.TargetX = x;
            _state.TargetY = y;
            _state.TargetZ = z;
        }

        public void SetFov(float fov) => _state.Fov = fov;
        public void SetDistance(float distance) => _state.Distance = distance;
        public void SetExtra(Dictionary<string, object> extra) => _state.Extra = extra;
    }
}
