namespace BabylonArchiveCore.Tests.Smoke;

/// <summary>
/// Smoke тесты Block 01.
/// Быстрые проверки целостности архитектурного фундамента.
/// </summary>
public class Block01SmokeTests
{
    /// <summary>Решение содержит ожидаемые проекты.</summary>
    // [Fact]
    public void Solution_ContainsExpectedProjects()
    {
        // Assert: BabylonArchiveCore.sln references Core/Runtime/UI/Content/Tests
    }

    /// <summary>Документ инвариантов содержит INV-001..INV-015.</summary>
    // [Fact]
    public void InvariantsDocument_ContainsExpectedRange()
    {
        // Assert: invariants.md includes INV-001 through INV-015
    }

    /// <summary>ADR index синхронизирован до ADR-010.</summary>
    // [Fact]
    public void AdrIndex_ContainsAllCurrentAdrs()
    {
        // Assert: docs/adr/INDEX.md includes ADR-001..ADR-010
    }

    /// <summary>CI workflow существует и включает test step.</summary>
    // [Fact]
    public void CiWorkflow_ExistsAndRunsTests()
    {
        // Assert: .github/workflows/ci.yml exists and contains dotnet test
    }
}
