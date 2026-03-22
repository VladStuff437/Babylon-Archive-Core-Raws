using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Plays a scripted camera sequence: a list of keyframes interpolated over time.
/// Used for the prologue flythrough before gameplay begins.
/// </summary>
public sealed class CutsceneSequencer
{
    private readonly List<CutsceneKeyframe> _keyframes;
    private float _elapsed;
    private int _currentIndex;

    public bool IsPlaying { get; private set; }
    public bool IsFinished { get; private set; }
    public float TotalDuration { get; }
    public float Elapsed => _elapsed;
    public Vec3 CurrentPosition { get; private set; }
    public Vec3 CurrentLookAt { get; private set; }

    public CutsceneSequencer(List<CutsceneKeyframe> keyframes)
    {
        if (keyframes.Count < 2)
            throw new ArgumentException("Cutscene requires at least 2 keyframes.", nameof(keyframes));

        _keyframes = keyframes;
        TotalDuration = _keyframes[^1].Time;
        CurrentPosition = _keyframes[0].Position;
        CurrentLookAt = _keyframes[0].LookAt;
    }

    public void Play()
    {
        IsPlaying = true;
        IsFinished = false;
        _elapsed = 0;
        _currentIndex = 0;
        CurrentPosition = _keyframes[0].Position;
        CurrentLookAt = _keyframes[0].LookAt;
    }

    /// <summary>
    /// Advance the cutscene by deltaTime seconds.
    /// Returns the interpolated camera position and look-at target.
    /// </summary>
    public CutsceneFrame Update(float deltaTime)
    {
        if (!IsPlaying || IsFinished)
            return new CutsceneFrame(CurrentPosition, CurrentLookAt, IsFinished);

        _elapsed += deltaTime;

        if (_elapsed >= TotalDuration)
        {
            _elapsed = TotalDuration;
            IsPlaying = false;
            IsFinished = true;
            CurrentPosition = _keyframes[^1].Position;
            CurrentLookAt = _keyframes[^1].LookAt;
            return new CutsceneFrame(CurrentPosition, CurrentLookAt, true);
        }

        // Find the two surrounding keyframes
        while (_currentIndex < _keyframes.Count - 2 && _keyframes[_currentIndex + 1].Time <= _elapsed)
            _currentIndex++;

        var from = _keyframes[_currentIndex];
        var to = _keyframes[_currentIndex + 1];
        var segmentDuration = to.Time - from.Time;
        var t = segmentDuration > 0 ? (_elapsed - from.Time) / segmentDuration : 1f;

        CurrentPosition = from.Position.Lerp(to.Position, t);
        CurrentLookAt = from.LookAt.Lerp(to.LookAt, t);

        return new CutsceneFrame(CurrentPosition, CurrentLookAt, false);
    }

    /// <summary>
    /// Skip to the end of the cutscene immediately.
    /// </summary>
    public void Skip()
    {
        _elapsed = TotalDuration;
        IsPlaying = false;
        IsFinished = true;
        CurrentPosition = _keyframes[^1].Position;
        CurrentLookAt = _keyframes[^1].LookAt;
    }
}

/// <summary>A keyframe in a cutscene: position, look-at, and time in seconds.</summary>
public readonly record struct CutsceneKeyframe(Vec3 Position, Vec3 LookAt, float Time);

/// <summary>Interpolated camera state at a point in the cutscene.</summary>
public readonly record struct CutsceneFrame(Vec3 Position, Vec3 LookAt, bool IsFinished);
