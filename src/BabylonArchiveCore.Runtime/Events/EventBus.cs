namespace BabylonArchiveCore.Runtime.Events;

/// <summary>
/// Базовый EventBus для runtime-событий S011-S020.
/// </summary>
public sealed class EventBus
{
    private readonly Dictionary<string, List<Action<object>>> subscriptions = new(StringComparer.Ordinal);

    public IReadOnlyCollection<string> KnownEvents => subscriptions.Keys.ToArray();

    public void Subscribe(string eventName, Action<object> handler)
    {
        if (!subscriptions.TryGetValue(eventName, out var handlers))
        {
            handlers = new List<Action<object>>();
            subscriptions[eventName] = handlers;
        }

        handlers.Add(handler);
    }

    public void Publish(string eventName, object payload)
    {
        if (!subscriptions.TryGetValue(eventName, out var handlers))
        {
            return;
        }

        foreach (var handler in handlers.ToArray())
        {
            handler(payload);
        }
    }

    public void RegisterSession014To016Events()
    {
        EnsureEvent(SessionEvents.Session014MigrationPipelineReady);
        EnsureEvent(SessionEvents.Session015TaxonomyApplied);
        EnsureEvent(SessionEvents.Session016GameLoopStarted);
    }

    public void RegisterSession017To019Events()
    {
        EnsureEvent(SessionEvents.Session017RuntimeStateSynchronized);
        EnsureEvent(SessionEvents.Session018CommandContourValidated);
        EnsureEvent(SessionEvents.Session019ActionMapResolved);
    }

    public void RegisterSession014To019Events()
    {
        RegisterSession014To016Events();
        RegisterSession017To019Events();
    }

    public void RegisterSession020Events()
    {
        EnsureEvent(SessionEvents.Session020ControlProfilesConsolidated);
    }

    public void RegisterSession014To020Events()
    {
        RegisterSession014To019Events();
        RegisterSession020Events();
    }

    private void EnsureEvent(string eventName)
    {
        if (!subscriptions.ContainsKey(eventName))
        {
            subscriptions[eventName] = new List<Action<object>>();
        }
    }
}
