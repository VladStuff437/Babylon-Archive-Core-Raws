namespace BabylonArchiveCore.Core.Gameplay.Interactions;

/// <summary>
/// Протокол интерактивного объекта мира.
/// </summary>
public interface IInteractable
{
    string InteractableId { get; }
    string InteractionType { get; } // Pickup, Talk, Use, Examine
    bool CanInteract(string actorId);
    void OnInteract(string actorId);
}
