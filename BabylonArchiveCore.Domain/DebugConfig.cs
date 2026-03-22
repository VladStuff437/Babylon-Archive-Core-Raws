namespace BabylonArchiveCore.Domain;

/// <summary>
/// Debug configuration for development builds.
/// Controls debug overlays, teleportation, and diagnostic output.
/// </summary>
public sealed class DebugConfig
{
    /// <summary>Master toggle for all debug features.</summary>
    public bool Enabled { get; set; }

    /// <summary>Show current zone, active triggers, focused object name.</summary>
    public bool ShowOverlay { get; set; }

    /// <summary>Allow instant zone teleportation via debug console.</summary>
    public bool AllowTeleport { get; set; }

    /// <summary>Log all trigger activations to console/journal.</summary>
    public bool LogTriggers { get; set; }

    /// <summary>Log all phase transitions to console/journal.</summary>
    public bool LogPhaseChanges { get; set; }

    /// <summary>Show FPS counter.</summary>
    public bool ShowFps { get; set; }

    /// <summary>Enable free camera mode toggle.</summary>
    public bool AllowFreeCamera { get; set; }

    public static DebugConfig Production() => new() { Enabled = false };

    public static DebugConfig Development() => new()
    {
        Enabled = true,
        ShowOverlay = true,
        AllowTeleport = true,
        LogTriggers = true,
        LogPhaseChanges = true,
        ShowFps = true,
        AllowFreeCamera = true,
    };
}
