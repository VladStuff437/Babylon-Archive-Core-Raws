namespace BabylonArchiveCore.Runtime.Combat;

/// <summary>
/// Режим автоатаки с безопасной остановкой.
/// </summary>
public sealed class AutoAttackController
{
    private int cooldownRemainingTicks;
    private bool missionTransitionLocked;

    public bool IsActive { get; private set; }
    public string? CurrentTargetId { get; private set; }
    public int AttackIntervalTicks { get; private set; } = 1;

    public bool IsMissionTransitionLocked => missionTransitionLocked;

    public void Start(string targetId)
    {
        ArgumentNullException.ThrowIfNull(targetId);
        CurrentTargetId = targetId;
        IsActive = true;
        cooldownRemainingTicks = 0;
    }

    public void Start(string targetId, int attackIntervalTicks)
    {
        ArgumentNullException.ThrowIfNull(targetId);

        AttackIntervalTicks = Math.Max(1, attackIntervalTicks);
        CurrentTargetId = targetId;
        IsActive = true;
        cooldownRemainingTicks = 0;
    }

    public void Stop()
    {
        IsActive = false;
        CurrentTargetId = null;
        cooldownRemainingTicks = 0;
    }

    public void BeginMissionTransitionLock()
    {
        missionTransitionLocked = true;
    }

    public void EndMissionTransitionLock()
    {
        missionTransitionLocked = false;
    }

    /// <summary>
    /// Тик автоатаки. Возвращает true, когда должен быть нанесён удар.
    /// </summary>
    public bool Tick()
    {
        if (!IsActive || CurrentTargetId is null || missionTransitionLocked)
        {
            return false;
        }

        if (cooldownRemainingTicks > 0)
        {
            cooldownRemainingTicks--;
            return false;
        }

        cooldownRemainingTicks = AttackIntervalTicks - 1;
        return true;
    }

    /// <summary>Безопасная остановка: всегда оставляет контроллер в валидном состоянии.</summary>
    public void SafeStop()
    {
        Stop();
    }
}
