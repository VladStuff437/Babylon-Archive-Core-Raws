using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Input;

/// <summary>
/// Обработчик команд с валидацией некорректных состояний.
/// </summary>
public sealed class CommandPipeline
{
    private readonly IWorldStateReader worldStateReader;
    private readonly IWorldStateMutator worldStateMutator;

    public CommandPipeline(IWorldStateReader worldStateReader, IWorldStateMutator worldStateMutator)
    {
        this.worldStateReader = worldStateReader;
        this.worldStateMutator = worldStateMutator;
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

        var success = worldStateMutator.ApplyCommand(command);
        error = success ? string.Empty : "Command was rejected by world mutator.";
        return success;
    }
}
