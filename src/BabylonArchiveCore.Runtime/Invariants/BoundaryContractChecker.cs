using BabylonArchiveCore.Core.Invariants;

namespace BabylonArchiveCore.Runtime.Invariants;

/// <summary>
/// INV-008: Boundary Contract Enforcement.
/// Проверяет что все boundary interfaces находятся в Core.Contracts namespace.
/// </summary>
public sealed class BoundaryContractChecker : IInvariantChecker
{
    public string InvariantId => "INV-008";
    public string Description => "Все cross-layer контракты в Core.Contracts namespace";
    public InvariantSeverity Severity => InvariantSeverity.Critical;

    private static readonly string[] BoundaryInterfaceNames =
    {
        "IWorldStateReader", "IWorldStateMutator", "ICommand",
        "ICommandDispatcher", "IGenerator", "IContentLoader",
        "IViewStateProvider", "IInputHandler"
    };

    public InvariantResult Check()
    {
        var coreAssembly = typeof(IInvariantChecker).Assembly;
        var contractTypes = coreAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Namespace == "BabylonArchiveCore.Core.Contracts")
            .Select(t => t.Name)
            .ToHashSet();

        var missing = BoundaryInterfaceNames
            .Where(name => !contractTypes.Contains(name))
            .ToList();

        if (missing.Count > 0)
        {
            return InvariantResult.Fail(InvariantId,
                $"Boundary interfaces отсутствуют в Core.Contracts: {string.Join(", ", missing)}");
        }

        return InvariantResult.Pass(InvariantId,
            $"Все {BoundaryInterfaceNames.Length} boundary interfaces в Core.Contracts");
    }
}
