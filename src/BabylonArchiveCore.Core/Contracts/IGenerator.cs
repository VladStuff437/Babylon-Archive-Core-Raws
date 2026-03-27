namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт seed-based генерации.
/// Boundary contract: Core → Runtime (INV-003, ADR-003).
/// </summary>
public interface IGenerator
{
    /// <summary>Seed, используемый для генерации.</summary>
    long Seed { get; }

    /// <summary>Сгенерировать целое число в диапазоне [min, max).</summary>
    int NextInt(int min, int max);

    /// <summary>Сгенерировать дочерний seed для подуровня.</summary>
    long DeriveChildSeed(string key);
}
