namespace BabylonArchiveCore.Runtime.Combat.StatusEffects;

/// <summary>
/// Менеджер статусных эффектов: тик и снятие истёкших.
/// </summary>
public sealed class StatusEffectManager
{
    private readonly List<StatusEffect> _effects = new();

    public IReadOnlyList<StatusEffect> ActiveEffects => _effects.Where(e => !e.IsExpired).ToList().AsReadOnly();

    public void Apply(StatusEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);

        var existing = _effects.FirstOrDefault(e => e.EffectId == effect.EffectId && !e.IsExpired);
        if (existing is not null)
        {
            if (!existing.TryAddStack())
            {
                existing.RemainingTicks = Math.Max(existing.RemainingTicks, effect.RemainingTicks);
            }

            return;
        }

        _effects.Add(effect);
    }

    /// <summary>Тик всех эффектов, возвращает суммарный урон.</summary>
    public int TickAll()
    {
        int totalDamage = 0;
        foreach (var effect in _effects)
        {
            totalDamage += effect.Tick();
        }
        _effects.RemoveAll(e => e.IsExpired);
        return totalDamage;
    }

    public bool RemoveById(string effectId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(effectId);
        return _effects.RemoveAll(e => string.Equals(e.EffectId, effectId, StringComparison.Ordinal)) > 0;
    }

    public int RemoveByCategory(string category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        return _effects.RemoveAll(e => string.Equals(e.Category, category, StringComparison.Ordinal));
    }

    public bool HasEffect(string effectId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(effectId);
        return _effects.Any(e => !e.IsExpired && string.Equals(e.EffectId, effectId, StringComparison.Ordinal));
    }

    public void Clear() => _effects.Clear();
}
