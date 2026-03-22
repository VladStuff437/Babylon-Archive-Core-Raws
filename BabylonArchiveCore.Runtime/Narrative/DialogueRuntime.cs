using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Dialogue;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Domain.World;
using BabylonArchiveCore.Runtime.Mission;

namespace BabylonArchiveCore.Runtime.Narrative;

/// <summary>
/// Executes a branching dialogue tree with multi-step skill checks.
/// Tracks current line, validates options against WorldState and player stats,
/// applies effects, and publishes check events.
/// </summary>
public sealed class DialogueRuntime
{
    private readonly DialogueDefinition _definition;
    private readonly WorldState _worldState;
    private readonly ILogger _logger;
    private readonly EventBus _eventBus;

    /// <summary>Player stat scores keyed by <see cref="DialogueCheckType"/> name.</summary>
    private readonly Dictionary<string, int> _playerStats;

    public string CurrentLineId { get; private set; }
    public bool IsFinished { get; private set; }
    public List<string> Transcript { get; } = new();

    public DialogueRuntime(
        DialogueDefinition definition,
        WorldState worldState,
        Dictionary<string, int> playerStats,
        ILogger logger,
        EventBus eventBus)
    {
        _definition = definition;
        _worldState = worldState;
        _playerStats = playerStats;
        _logger = logger;
        _eventBus = eventBus;
        CurrentLineId = definition.StartLineId;
    }

    public DialogueLine? CurrentLine =>
        _definition.Lines.TryGetValue(CurrentLineId, out var l) ? l : null;

    /// <summary>
    /// Begin the dialogue. Records the first line to the transcript.
    /// </summary>
    public bool Start()
    {
        if (CurrentLine is null)
        {
            _logger.Error($"Dialogue '{_definition.Id}': start line '{_definition.StartLineId}' not found.");
            return false;
        }

        var line = CurrentLine;
        RecordLine(line);
        _logger.Info($"Dialogue '{_definition.Id}' started: [{line.Speaker}] {line.Text}");

        if (line.IsTerminal)
            IsFinished = true;

        return true;
    }

    /// <summary>
    /// Returns options available to the player for the current line,
    /// filtered by WorldState flags and check feasibility.
    /// </summary>
    public List<DialogueOption> GetAvailableOptions()
    {
        if (IsFinished || CurrentLine is null) return new();

        var result = new List<DialogueOption>();
        foreach (var opt in CurrentLine.Options)
        {
            if (opt.RequiredFlag is not null && !_worldState.HasFlag(opt.RequiredFlag))
                continue;

            result.Add(opt);
        }
        return result;
    }

    /// <summary>
    /// Choose an option by id. Validates the option, performs skill check if needed,
    /// applies effects, advances to the target line.
    /// Returns true if the choice was accepted (check may still fail — see <see cref="LastCheckPassed"/>).
    /// </summary>
    public bool Choose(string optionId)
    {
        if (IsFinished) return false;

        var line = CurrentLine;
        if (line is null) return false;

        var option = line.Options.Find(o => o.Id == optionId);
        if (option is null)
        {
            _logger.Warn($"Dialogue '{_definition.Id}': option '{optionId}' not found in line '{CurrentLineId}'.");
            return false;
        }

        // Check flag requirement
        if (option.RequiredFlag is not null && !_worldState.HasFlag(option.RequiredFlag))
        {
            _logger.Warn($"Dialogue '{_definition.Id}': option '{optionId}' requires flag '{option.RequiredFlag}'.");
            return false;
        }

        // Perform skill check
        var checkPassed = true;
        if (option.CheckType != DialogueCheckType.None)
        {
            var statKey = option.CheckType.ToString();
            _playerStats.TryGetValue(statKey, out var playerStat);
            checkPassed = playerStat >= option.Difficulty;

            _eventBus.Publish(new DialogueCheckEvent
            {
                DialogueId = _definition.Id,
                OptionId = optionId,
                CheckType = statKey,
                Difficulty = option.Difficulty,
                PlayerStat = playerStat,
                Passed = checkPassed,
            });

            _logger.Info($"Dialogue check: {statKey} {playerStat}/{option.Difficulty} → {(checkPassed ? "PASS" : "FAIL")}");

            if (!checkPassed)
            {
                LastCheckPassed = false;
                return true; // Choice accepted but check failed — no transition
            }
        }

        LastCheckPassed = true;

        // Apply effects
        if (option.Effect is not null)
            WorldStateEffectApplier.Apply(_worldState, option.Effect, _logger);

        // Transition
        if (!_definition.Lines.ContainsKey(option.TargetLineId))
        {
            _logger.Error($"Dialogue '{_definition.Id}': target line '{option.TargetLineId}' not found.");
            return false;
        }

        _logger.Info($"Dialogue '{_definition.Id}': {CurrentLineId} --[{optionId}]--> {option.TargetLineId}");
        CurrentLineId = option.TargetLineId;

        var nextLine = CurrentLine!;
        RecordLine(nextLine);

        if (nextLine.IsTerminal)
        {
            IsFinished = true;
            _logger.Info($"Dialogue '{_definition.Id}' finished at line '{CurrentLineId}'.");
        }

        return true;
    }

    /// <summary>Whether the last skill check passed (true if no check was required).</summary>
    public bool LastCheckPassed { get; private set; } = true;

    private void RecordLine(DialogueLine line) =>
        Transcript.Add($"[{line.Speaker}] {line.Text}");
}
