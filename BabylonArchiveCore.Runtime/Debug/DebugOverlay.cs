using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Debug;

/// <summary>
/// Collects and formats debug information for the current frame.
/// Only populates when <see cref="DebugConfig.ShowOverlay"/> is true.
/// </summary>
public sealed class DebugOverlay
{
    private readonly DebugConfig _config;
    private int _frameCounter;
    private float _fpsAccumulator;
    private float _currentFps;

    public DebugConfig Config => _config;
    public float CurrentFps => _currentFps;

    public DebugOverlay(DebugConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Build an overlay snapshot for the current frame.
    /// Returns null if debug overlay is disabled.
    /// </summary>
    public DebugOverlaySnapshot? Update(
        float deltaTime,
        HubZoneId? currentZone,
        HubRhythmPhase phase,
        string? focusObjectId,
        int triggersFired,
        Vec3 playerPosition)
    {
        if (!_config.Enabled || !_config.ShowOverlay)
            return null;

        // FPS calculation (rolling average over 60 frames)
        _frameCounter++;
        _fpsAccumulator += deltaTime;
        if (_frameCounter >= 60)
        {
            _currentFps = _fpsAccumulator > 0 ? _frameCounter / _fpsAccumulator : 0;
            _frameCounter = 0;
            _fpsAccumulator = 0;
        }

        return new DebugOverlaySnapshot
        {
            Zone = currentZone,
            Phase = phase,
            FocusObject = focusObjectId,
            TriggersFired = triggersFired,
            Fps = _config.ShowFps ? _currentFps : null,
            PlayerPosition = playerPosition,
        };
    }
}

/// <summary>Immutable snapshot of debug overlay state for rendering.</summary>
public sealed record DebugOverlaySnapshot
{
    public HubZoneId? Zone { get; init; }
    public HubRhythmPhase Phase { get; init; }
    public string? FocusObject { get; init; }
    public int TriggersFired { get; init; }
    public float? Fps { get; init; }
    public Vec3 PlayerPosition { get; init; }

    public string Format()
    {
        var lines = new List<string>
        {
            $"Zone: {Zone?.ToString() ?? "—"}",
            $"Phase: {Phase}",
            $"Focus: {FocusObject ?? "—"}",
            $"Triggers: {TriggersFired}",
            $"Pos: ({PlayerPosition.X:F1}, {PlayerPosition.Z:F1})",
        };
        if (Fps.HasValue)
            lines.Add($"FPS: {Fps.Value:F0}");
        return string.Join(" | ", lines);
    }
}
