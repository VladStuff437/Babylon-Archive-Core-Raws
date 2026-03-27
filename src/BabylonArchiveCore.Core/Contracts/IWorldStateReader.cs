namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Read-only доступ к WorldState. Используется Runtime и проецируется в UI.
/// Boundary contract: Core → Runtime (INV-002, ADR-003).
/// </summary>
public interface IWorldStateReader
{
    /// <summary>Текущий тик/шаг мира.</summary>
    long WorldTick { get; }

    /// <summary>Seed текущего мира.</summary>
    long WorldSeed { get; }

    /// <summary>Текущий игровой режим (Exploration / Combat).</summary>
    string CurrentMode { get; }

    /// <summary>True если WorldState инициализирован и валиден.</summary>
    bool IsInitialized { get; }
}
