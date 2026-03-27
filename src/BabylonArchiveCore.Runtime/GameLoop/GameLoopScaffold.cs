namespace BabylonArchiveCore.Runtime.GameLoop;

/// <summary>
/// Минимальный каркас game loop для S016.
/// </summary>
public sealed class GameLoopScaffold
{
    public long Tick { get; private set; }

    public bool IsPaused { get; private set; }

    public void Update()
    {
        if (IsPaused)
        {
            return;
        }

        Tick++;
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Resume()
    {
        IsPaused = false;
    }
}
