using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Resolves interactions by InteractiveType. Dispatches triggers, terminals, NPCs, gates.
/// Coordinates with DialoguePlayer, ObjectiveTracker, TriggerSystem, and PlayerInventory.
/// </summary>
public sealed class InteractionHandler
{
    private readonly DialoguePlayer _dialoguePlayer;
    private readonly ObjectiveTracker _objectiveTracker;
    private readonly TriggerSystem _triggerSystem;
    private readonly PlayerInventory _inventory;
    private readonly EventJournal _journal;
    private readonly ILogger _logger;
    private readonly Dictionary<string, TerminalScreen> _terminalScreens = new();
    private readonly Dictionary<string, List<InventoryItem>> _itemGrants = new();
    private readonly Dictionary<string, IReadOnlyList<TriggerAction>> _objectInteractionActions = new(StringComparer.OrdinalIgnoreCase);

    public InteractionHandler(
        DialoguePlayer dialoguePlayer,
        ObjectiveTracker objectiveTracker,
        TriggerSystem triggerSystem,
        PlayerInventory inventory,
        EventJournal journal,
        ILogger logger)
    {
        _dialoguePlayer = dialoguePlayer;
        _objectiveTracker = objectiveTracker;
        _triggerSystem = triggerSystem;
        _inventory = inventory;
        _journal = journal;
        _logger = logger;
    }

    public void RegisterTerminalScreen(TerminalScreen screen) =>
        _terminalScreens[screen.TerminalId] = screen;

    public void RegisterItemGrant(string objectId, List<InventoryItem> items) =>
        _itemGrants[objectId] = items;

    public void RegisterObjectInteractionActions(string objectId, IReadOnlyList<TriggerAction> actions) =>
        _objectInteractionActions[objectId] = actions;

    /// <summary>
    /// Handle interaction based on object type and return enriched result.
    /// The HubA0Runtime still manages phase advancement; this handler adds
    /// dialogue, inventory, terminal, and trigger processing.
    /// </summary>
    public InteractionResult Handle(InteractableObject obj, InteractionResult baseResult)
    {
        string? dialogueId = null;
        IReadOnlyList<InventoryItem>? grantedItems = null;
        TerminalScreen? terminalScreen = null;

        switch (obj.InteractiveType)
        {
            case InteractiveType.Trigger:
                // Instant action — base result is sufficient
                break;

            case InteractiveType.Terminal:
                // Show terminal screen + optional dialogue
                if (_terminalScreens.TryGetValue(obj.Id, out var screen))
                    terminalScreen = screen;

                dialogueId = obj.DialogueId;
                if (dialogueId is not null)
                    _dialoguePlayer.Start(dialogueId);

                // Grant items if this terminal grants items
                grantedItems = GrantItems(obj.Id);
                break;

            case InteractiveType.Npc:
                // Dialogue encounter
                dialogueId = obj.DialogueId;
                if (dialogueId is not null)
                    _dialoguePlayer.Start(dialogueId);
                break;

            case InteractiveType.Gate:
                // Locked gate — show locked message via dialogue if available
                dialogueId = obj.DialogueId;
                if (dialogueId is not null)
                    _dialoguePlayer.Start(dialogueId);
                break;
        }

        // Fire phase triggers if phase advanced
        if (baseResult.NewPhase is { } newPhase)
        {
            var triggerResult = _triggerSystem.OnPhaseChanged(newPhase);
            foreach (var action in triggerResult.FiredActions)
            {
                switch (action.Type)
                {
                    case TriggerActionType.SetObjective:
                        if (action.ObjectiveId is not null)
                        {
                            // Complete the previous active objective
                            var activeObj = _objectiveTracker.GetActive();
                            if (activeObj is not null)
                                _objectiveTracker.Complete(activeObj.ObjectiveId);

                            _objectiveTracker.SetActive(action.ObjectiveId);
                        }
                        break;

                    case TriggerActionType.PlayDialogue:
                        if (action.DialogueId is not null && dialogueId is null)
                        {
                            dialogueId = action.DialogueId;
                            _dialoguePlayer.Start(action.DialogueId);
                        }
                        break;

                    case TriggerActionType.JournalEntry:
                        if (action.Text is not null)
                            _journal.Record("trigger", action.Text);
                        break;
                }
            }
        }

        if (_objectInteractionActions.TryGetValue(obj.Id, out var scriptedActions))
        {
            foreach (var action in scriptedActions)
            {
                switch (action.Type)
                {
                    case TriggerActionType.SetObjective:
                        if (action.ObjectiveId is not null)
                            _objectiveTracker.SetActive(action.ObjectiveId);
                        break;

                    case TriggerActionType.PlayDialogue:
                        if (action.DialogueId is not null)
                        {
                            dialogueId = action.DialogueId;
                            _dialoguePlayer.Start(action.DialogueId);
                        }
                        break;

                    case TriggerActionType.JournalEntry:
                        if (!string.IsNullOrWhiteSpace(action.Text))
                            _journal.Record("trigger", action.Text);
                        break;
                }
            }
        }

        _logger.Info($"InteractionHandler: {obj.Id} ({obj.InteractiveType}) → dialogue={dialogueId}, items={grantedItems?.Count ?? 0}");

        return new InteractionResult
        {
            ObjectId = baseResult.ObjectId,
            Success = baseResult.Success,
            Message = baseResult.Message,
            NewPhase = baseResult.NewPhase,
            ObjectType = obj.InteractiveType,
            DialogueId = dialogueId,
            GrantedItems = grantedItems,
            TerminalScreen = terminalScreen,
        };
    }

    private IReadOnlyList<InventoryItem>? GrantItems(string objectId)
    {
        if (!_itemGrants.TryGetValue(objectId, out var items))
            return null;

        foreach (var item in items)
            _inventory.Add(item);

        return items;
    }
}
