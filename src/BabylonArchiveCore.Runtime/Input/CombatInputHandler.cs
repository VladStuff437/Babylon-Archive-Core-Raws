namespace BabylonArchiveCore.Runtime.Input;

/// <summary>
/// Обработчик боевого ввода: Tab-таргет и Esc-cancel.
/// </summary>
public sealed class CombatInputHandler
{
    private readonly List<string> _targets = new();
    private int _currentIndex = -1;
    private bool _isMissionTransitionLocked;

    public string? CurrentTarget => _currentIndex >= 0 && _currentIndex < _targets.Count ? _targets[_currentIndex] : null;

    public bool IsMissionTransitionLocked => _isMissionTransitionLocked;

    public void SetTargets(IEnumerable<string> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        _targets.Clear();
        _targets.AddRange(targets);
        _currentIndex = _targets.Count > 0 ? 0 : -1;
    }

    public void BeginMissionTransitionLock()
    {
        _isMissionTransitionLocked = true;
    }

    public void EndMissionTransitionLock()
    {
        _isMissionTransitionLocked = false;
    }

    /// <summary>Tab-таргет: переключить на следующую цель.</summary>
    public string? TabTarget()
    {
        if (_isMissionTransitionLocked)
        {
            return CurrentTarget;
        }

        if (_targets.Count == 0) return null;
        _currentIndex = (_currentIndex + 1) % _targets.Count;
        return CurrentTarget;
    }

    /// <summary>Esc-cancel: сбросить выбор цели.</summary>
    public void EscCancel()
    {
        if (_isMissionTransitionLocked)
        {
            return;
        }

        _currentIndex = -1;
    }
}
