using BabylonArchiveCore.Core.Invariants;

namespace BabylonArchiveCore.Runtime.Invariants;

/// <summary>
/// INV-002: Однонаправленные зависимости слоёв.
/// Проверяет что граф ProjectReference ацикличен и соответствует правилам.
/// В runtime проверяет assembly references.
/// </summary>
public sealed class LayerDependencyChecker : IInvariantChecker
{
    public string InvariantId => "INV-002";
    public string Description => "Однонаправленные зависимости: Core ← Runtime ← UI";
    public InvariantSeverity Severity => InvariantSeverity.Critical;

    public InvariantResult Check()
    {
        // Проверяем что Core assembly не ссылается на Runtime или UI
        var coreAssembly = typeof(IInvariantChecker).Assembly;
        var coreRefs = coreAssembly.GetReferencedAssemblies();

        foreach (var refName in coreRefs)
        {
            if (refName.Name is not null &&
                (refName.Name.Contains("Runtime", StringComparison.OrdinalIgnoreCase) ||
                 refName.Name.Contains(".UI", StringComparison.OrdinalIgnoreCase)))
            {
                return InvariantResult.Fail(InvariantId,
                    $"Core ссылается на запрещённую сборку: {refName.Name}");
            }
        }

        return InvariantResult.Pass(InvariantId,
            "Core assembly не содержит запрещённых ссылок");
    }
}
