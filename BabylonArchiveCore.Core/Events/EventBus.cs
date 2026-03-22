namespace BabylonArchiveCore.Core.Events;

public sealed class EventBus
{
    private readonly Dictionary<Type, List<Delegate>> _subscriptions = new();

    public void Subscribe<TEvent>(Action<TEvent> handler)
        where TEvent : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(handler);

        var eventType = typeof(TEvent);

        if (!_subscriptions.TryGetValue(eventType, out var handlers))
        {
            handlers = new List<Delegate>();
            _subscriptions[eventType] = handlers;
        }

        handlers.Add(handler);
    }

    public void Publish<TEvent>(TEvent domainEvent)
        where TEvent : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var eventType = typeof(TEvent);

        if (!_subscriptions.TryGetValue(eventType, out var handlers))
        {
            return;
        }

        foreach (var handler in handlers.Cast<Action<TEvent>>())
        {
            handler(domainEvent);
        }
    }
}
