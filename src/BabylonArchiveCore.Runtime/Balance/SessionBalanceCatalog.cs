namespace BabylonArchiveCore.Runtime.Balance;

/// <summary>
/// Каталог баланс-профилей по идентификатору сессии.
/// </summary>
public sealed class SessionBalanceCatalog
{
    private readonly Dictionary<string, BalanceTable> tables = new(StringComparer.Ordinal);

    public void Register(string sessionId, BalanceTable table)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        ArgumentNullException.ThrowIfNull(table);

        tables[sessionId] = table;
    }

    public bool TryGet(string sessionId, out BalanceTable? table)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        return tables.TryGetValue(sessionId, out table);
    }

    public BalanceTable RegisterFromJson(string sessionId, string json, BalanceTableLoader loader)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        ArgumentNullException.ThrowIfNull(loader);

        var table = loader.LoadFromJson(json);
        Register(sessionId, table);
        return table;
    }

    public BalanceTable GetRequired(string sessionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        if (!tables.TryGetValue(sessionId, out var table))
        {
            throw new KeyNotFoundException($"Balance table for session '{sessionId}' is not registered.");
        }

        return table;
    }
}
