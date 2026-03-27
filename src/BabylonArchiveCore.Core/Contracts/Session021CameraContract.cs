using System;
using System.Collections.Generic;

namespace BabylonArchiveCore.Core.Contracts
{
    /// <summary>
    /// Контракт состояния и режимов камеры для сессии 021 (Follow/Aim/Inspect).
    /// </summary>
    public sealed class Session021CameraContract
    {
        public string Mode { get; set; } = "Follow"; // Follow, Aim, Inspect
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float TargetX { get; set; }
        public float TargetY { get; set; }
        public float TargetZ { get; set; }
        public float Fov { get; set; } = 60f;
        public float Distance { get; set; } = 5f;
        public Dictionary<string, object>? Extra { get; set; }
    }
}
