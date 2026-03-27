using BabylonArchiveCore.Core.Gameplay.Interactions;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Runtime-исполнитель взаимодействий, связывающий IInteractable с геймплейным контуром.
/// </summary>
public sealed class InteractionExecutor
{
    private readonly List<IInteractable> _registry = new();

    public void Register(IInteractable interactable)
    {
        ArgumentNullException.ThrowIfNull(interactable);
        _registry.Add(interactable);
    }

    public bool TryInteract(string actorId, string targetId)
    {
        var target = _registry.Find(i => i.InteractableId == targetId);
        if (target is null || !target.CanInteract(actorId))
            return false;
        target.OnInteract(actorId);
        return true;
    }

    public IReadOnlyList<IInteractable> GetAll() => _registry.AsReadOnly();
}
