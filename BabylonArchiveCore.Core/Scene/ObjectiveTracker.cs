using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Core.Scene;

/// <summary>
/// Tracks ordered objectives. Activates, completes, and queries objective state.
/// </summary>
public sealed class ObjectiveTracker
{
    private readonly Dictionary<string, Objective> _objectives = new();

    public IReadOnlyDictionary<string, Objective> Objectives => _objectives;

    public void Register(Objective objective) =>
        _objectives[objective.ObjectiveId] = objective;

    public void SetActive(string objectiveId)
    {
        if (_objectives.TryGetValue(objectiveId, out var obj) && obj.Status == ObjectiveStatus.Locked)
            obj.Status = ObjectiveStatus.Active;
    }

    public void Complete(string objectiveId)
    {
        if (_objectives.TryGetValue(objectiveId, out var obj) && obj.Status == ObjectiveStatus.Active)
            obj.Status = ObjectiveStatus.Completed;
    }

    public Objective? GetActive() =>
        _objectives.Values
            .Where(o => o.Status == ObjectiveStatus.Active)
            .OrderBy(o => o.Order)
            .FirstOrDefault();

    public IReadOnlyList<Objective> GetAll() =>
        _objectives.Values.OrderBy(o => o.Order).ToList();

    public IReadOnlyList<Objective> GetCompleted() =>
        _objectives.Values.Where(o => o.Status == ObjectiveStatus.Completed).OrderBy(o => o.Order).ToList();
}
