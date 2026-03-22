using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Mission;

namespace BabylonArchiveCore.Runtime.Mission;

/// <summary>
/// Executes a mission graph: tracks the current node, processes player choices
/// (transition labels), and resolves terminal nodes into success/failure.
/// </summary>
public sealed class MissionGraphRuntime
{
    private readonly MissionDefinition _definition;
    private readonly ILogger _logger;
    private readonly EventBus _eventBus;

    public MissionStatus Status { get; private set; } = MissionStatus.NotStarted;
    public string CurrentNodeId { get; private set; }

    public MissionGraphRuntime(MissionDefinition definition, ILogger logger, EventBus eventBus)
    {
        _definition = definition;
        _logger = logger;
        _eventBus = eventBus;
        CurrentNodeId = definition.StartNodeId;
    }

    public MissionNode? CurrentNode =>
        _definition.Nodes.TryGetValue(CurrentNodeId, out var n) ? n : null;

    /// <summary>
    /// Start the mission. Transitions status to Active.
    /// </summary>
    public bool Start()
    {
        if (Status != MissionStatus.NotStarted) return false;
        if (CurrentNode is null)
        {
            _logger.Error($"Mission '{_definition.Id}': start node '{_definition.StartNodeId}' not found.");
            return false;
        }

        SetStatus(MissionStatus.Active);
        _logger.Info($"Mission '{_definition.Title}' started at node '{CurrentNodeId}'.");
        return true;
    }

    /// <summary>
    /// Process a player choice. The <paramref name="choiceLabel"/> must match
    /// one of the current node's transition keys.
    /// </summary>
    public bool Choose(string choiceLabel)
    {
        if (Status != MissionStatus.Active) return false;

        var node = CurrentNode;
        if (node is null) return false;

        if (!node.Transitions.TryGetValue(choiceLabel, out var nextId))
        {
            _logger.Warn($"Mission '{_definition.Id}': no transition '{choiceLabel}' from node '{CurrentNodeId}'.");
            return false;
        }

        if (!_definition.Nodes.ContainsKey(nextId))
        {
            _logger.Error($"Mission '{_definition.Id}': transition target '{nextId}' does not exist.");
            return false;
        }

        _logger.Info($"Mission '{_definition.Id}': {CurrentNodeId} --[{choiceLabel}]--> {nextId}");
        CurrentNodeId = nextId;

        var next = CurrentNode!;
        if (next.IsTerminalSuccess)
            SetStatus(MissionStatus.Completed);
        else if (next.IsTerminalFailure)
            SetStatus(MissionStatus.Failed);

        return true;
    }

    /// <summary>
    /// Force-fail the mission (e.g. timeout).
    /// </summary>
    public void Fail()
    {
        if (Status == MissionStatus.Active)
            SetStatus(MissionStatus.Failed);
    }

    /// <summary>
    /// Returns the effect to apply based on current terminal status, or null if not terminal.
    /// </summary>
    public MissionEffect? GetCompletionEffect() => Status switch
    {
        MissionStatus.Completed => _definition.OnSuccess,
        MissionStatus.Failed => _definition.OnFailure,
        _ => null,
    };

    private void SetStatus(MissionStatus newStatus)
    {
        var old = Status;
        Status = newStatus;
        _eventBus.Publish(new MissionStatusChangedEvent
        {
            MissionId = _definition.Id,
            OldStatus = old,
            NewStatus = newStatus,
        });
    }
}
