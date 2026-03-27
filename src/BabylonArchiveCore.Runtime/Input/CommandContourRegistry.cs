using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Input;

/// <summary>
/// Реестр правил выполнения команд для S018.
/// </summary>
public sealed class CommandContourRegistry
{
    private readonly Dictionary<string, Session018CommandContourContract> contracts = new(StringComparer.OrdinalIgnoreCase);

    public void Register(Session018CommandContourContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);

        contracts[contract.CommandType] = contract;
    }

    public bool Validate(ICommand command, IWorldStateReader worldStateReader, out string error)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(worldStateReader);

        if (!contracts.TryGetValue(command.CommandType, out var contour))
        {
            error = string.Empty;
            return true;
        }

        if (contour.RequiresInitializedWorld && !worldStateReader.IsInitialized)
        {
            error = "Command contour requires initialized world state.";
            return false;
        }

        if (!contour.AllowedModes.Contains(worldStateReader.CurrentMode, StringComparer.OrdinalIgnoreCase))
        {
            error = $"Command type '{command.CommandType}' is not allowed in mode '{worldStateReader.CurrentMode}'.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
