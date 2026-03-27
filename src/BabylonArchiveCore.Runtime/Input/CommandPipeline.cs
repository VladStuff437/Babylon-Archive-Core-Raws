using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Input;

/// <summary>
/// Обработчик команд с валидацией некорректных состояний.
/// </summary>
public sealed class CommandPipeline
{
    private readonly IWorldStateReader worldStateReader;
    private readonly IWorldStateMutator worldStateMutator;
    private readonly CommandContourRegistry commandContourRegistry;
    private readonly ActionMapCatalog actionMapCatalog;
    private readonly ControlProfileResolver controlProfileResolver;

    public CommandPipeline(IWorldStateReader worldStateReader, IWorldStateMutator worldStateMutator)
        : this(worldStateReader, worldStateMutator, new CommandContourRegistry(), new ActionMapCatalog(), new ControlProfileResolver())
    {
    }

    public CommandPipeline(
        IWorldStateReader worldStateReader,
        IWorldStateMutator worldStateMutator,
        CommandContourRegistry commandContourRegistry,
        ActionMapCatalog actionMapCatalog,
        ControlProfileResolver controlProfileResolver)
    {
        ArgumentNullException.ThrowIfNull(worldStateReader);
        ArgumentNullException.ThrowIfNull(worldStateMutator);
        ArgumentNullException.ThrowIfNull(commandContourRegistry);
        ArgumentNullException.ThrowIfNull(actionMapCatalog);
        ArgumentNullException.ThrowIfNull(controlProfileResolver);

        this.worldStateReader = worldStateReader;
        this.worldStateMutator = worldStateMutator;
        this.commandContourRegistry = commandContourRegistry;
        this.actionMapCatalog = actionMapCatalog;
        this.controlProfileResolver = controlProfileResolver;
    }

    public bool TryExecute(ICommand command, out string error)
    {
        if (command is null)
        {
            error = "Command is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(command.CommandType))
        {
            error = "Command type is required.";
            return false;
        }

        if (!worldStateReader.IsInitialized)
        {
            error = "WorldState is not initialized.";
            return false;
        }

        if (command.CreatedAtTick > worldStateReader.WorldTick + 1)
        {
            error = "Command tick is outside allowable execution window.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(worldStateReader.CurrentMode))
        {
            error = "Current world mode is invalid for command execution.";
            return false;
        }

        if (!commandContourRegistry.Validate(command, worldStateReader, out error))
        {
            return false;
        }

        var success = worldStateMutator.ApplyCommand(command);
        error = success ? string.Empty : "Command was rejected by world mutator.";
        return success;
    }

    public bool TryExecuteMappedInput(string profileId, string inputToken, long createdAtTick, out string error)
    {
        if (string.IsNullOrWhiteSpace(profileId))
        {
            error = "Profile id is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(inputToken))
        {
            error = "Input token is required.";
            return false;
        }

        if (!actionMapCatalog.TryResolveAction(profileId, inputToken, out var action))
        {
            error = "Input token is not mapped for the requested profile.";
            return false;
        }

        return TryExecute(new MappedInputCommand(action, createdAtTick), out error);
    }

    public bool TryExecuteWithProfileChain(
        string activeProfileId,
        string fallbackProfileId,
        IReadOnlyCollection<string> availableProfiles,
        string inputToken,
        long createdAtTick,
        out string error)
    {
        if (string.IsNullOrWhiteSpace(activeProfileId))
        {
            error = "Active profile id is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(fallbackProfileId))
        {
            error = "Fallback profile id is required.";
            return false;
        }

        if (availableProfiles is null || availableProfiles.Count == 0)
        {
            error = "At least one available profile is required.";
            return false;
        }

        var resolvedProfile = controlProfileResolver.Resolve(activeProfileId, fallbackProfileId, availableProfiles, strictResolution: false);
        return TryExecuteMappedInput(resolvedProfile, inputToken, createdAtTick, out error);
    }

    private sealed class MappedInputCommand : ICommand
    {
        public MappedInputCommand(string commandType, long createdAtTick)
        {
            CommandType = commandType;
            CreatedAtTick = createdAtTick;
        }

        public string CommandType { get; }

        public long CreatedAtTick { get; }
    }
}
