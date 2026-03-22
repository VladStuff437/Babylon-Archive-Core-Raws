using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Runtime.Events;

namespace BabylonArchiveCore.Runtime;

public sealed class ApplicationRuntime(
    GameConfiguration config,
    ILogger logger,
    GameStateRouter router,
    EventBus eventBus)
{
    public void RunDayOneBootstrapLoop(int ticks = 2)
    {
        logger.Info("Runtime bootstrap started.");

        if (!router.TrySwitch(SceneId.Boot))
        {
            logger.Error("Boot state is not registered.");
            return;
        }

        for (var i = 0; i < ticks; i++)
        {
            router.UpdateCurrent();
            Thread.Sleep(config.TickDelayMs);
        }

        var from = router.CurrentState?.Id ?? SceneId.Boot;
        if (router.TrySwitch(SceneId.HubA0))
        {
            eventBus.Publish(new SceneChangedEvent
            {
                From = from,
                To = SceneId.HubA0,
            });
        }

        for (var i = 0; i < ticks; i++)
        {
            router.UpdateCurrent();
            Thread.Sleep(config.TickDelayMs);
        }

        logger.Info("Runtime bootstrap finished.");
    }
}
