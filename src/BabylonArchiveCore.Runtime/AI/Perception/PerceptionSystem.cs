namespace BabylonArchiveCore.Runtime.AI.Perception;

/// <summary>
/// AI-система перцепции: обнаружение целей в радиусе.
/// </summary>
public sealed class PerceptionSystem
{
    public float DetectionRadius { get; set; } = 10f;
    public float AlertThreshold { get; set; } = 0.4f;

    /// <summary>Фильтрация целей по радиусу (упрощённая 1D-дистанция).</summary>
    public IReadOnlyList<string> DetectTargets(float selfPosition, IEnumerable<(string Id, float Position)> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        var results = new List<string>();
        foreach (var (id, pos) in candidates)
        {
            if (Math.Abs(pos - selfPosition) <= DetectionRadius)
                results.Add(id);
        }
        return results.AsReadOnly();
    }

    /// <summary>
    /// Расширенная фильтрация по радиусу и awareness score для S029.
    /// </summary>
    public IReadOnlyList<string> DetectTargetsWithAwareness(float selfPosition, IEnumerable<(string Id, float Position, float Awareness)> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        var results = new List<string>();
        foreach (var (id, position, awareness) in candidates)
        {
            if (awareness < AlertThreshold)
            {
                continue;
            }

            if (Math.Abs(position - selfPosition) <= DetectionRadius)
            {
                results.Add(id);
            }
        }

        return results.AsReadOnly();
    }

    public string? SelectPrimaryTarget(IReadOnlyList<string> detectedTargets, string? previousTargetId)
    {
        ArgumentNullException.ThrowIfNull(detectedTargets);

        if (!string.IsNullOrWhiteSpace(previousTargetId) && detectedTargets.Contains(previousTargetId, StringComparer.Ordinal))
        {
            return previousTargetId;
        }

        return detectedTargets.Count == 0 ? null : detectedTargets[0];
    }
}
