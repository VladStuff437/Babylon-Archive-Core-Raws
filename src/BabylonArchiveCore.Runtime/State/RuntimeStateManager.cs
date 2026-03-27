using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.State;

/// <summary>
/// Управление runtime-состоянием между тиками и режимами.
/// </summary>
public sealed class RuntimeStateManager
{
    private readonly IWorldStateReader worldStateReader;

    public RuntimeStateManager(IWorldStateReader worldStateReader)
    {
        ArgumentNullException.ThrowIfNull(worldStateReader);

        this.worldStateReader = worldStateReader;
        CurrentMode = string.IsNullOrWhiteSpace(worldStateReader.CurrentMode) ? "exploration" : worldStateReader.CurrentMode;
        LastAppliedTick = worldStateReader.WorldTick;
    }

    public string CurrentMode { get; private set; }

    public long LastAppliedTick { get; private set; }

    public bool IsDirty { get; private set; }

    public void MarkDirty()
    {
        IsDirty = true;
    }

    public void SyncFromWorld()
    {
        if (!worldStateReader.IsInitialized)
        {
            IsDirty = true;
            return;
        }

        CurrentMode = string.IsNullOrWhiteSpace(worldStateReader.CurrentMode) ? CurrentMode : worldStateReader.CurrentMode;
        LastAppliedTick = worldStateReader.WorldTick;
        IsDirty = false;
    }

    public bool TryTransition(string nextMode, out string error)
    {
        if (string.IsNullOrWhiteSpace(nextMode))
        {
            error = "Target mode is required.";
            return false;
        }

        if (string.Equals(CurrentMode, nextMode, StringComparison.OrdinalIgnoreCase))
        {
            error = "Runtime state is already in the requested mode.";
            return false;
        }

        CurrentMode = nextMode;
        IsDirty = true;
        error = string.Empty;
        return true;
    }
}
