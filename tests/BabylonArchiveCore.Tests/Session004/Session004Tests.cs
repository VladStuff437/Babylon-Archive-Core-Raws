namespace BabylonArchiveCore.Tests.Session004;

/// <summary>
/// Тесты сессии 004: Слои архитектуры (Layer Structure).
/// Проверяют правильность зависимостей между проектами.
/// </summary>
public class Session004Tests
{
    // === INV-002: Однонаправленные зависимости ===

    /// <summary>Core.csproj не содержит ProjectReference.</summary>
    // [Fact]
    public void CoreProject_HasNoProjectReferences()
    {
        // Arrange: load Core.csproj
        // Assert: zero ProjectReference elements
    }

    /// <summary>Runtime.csproj ссылается только на Core.</summary>
    // [Fact]
    public void RuntimeProject_ReferencesOnlyCore()
    {
        // Arrange: load Runtime.csproj
        // Assert: exactly one ProjectReference → Core
    }

    /// <summary>UI.csproj ссылается на Runtime (и транзитивно Core).</summary>
    // [Fact]
    public void UIProject_ReferencesRuntime()
    {
        // Arrange: load UI.csproj
        // Assert: ProjectReference → Runtime
        // Assert: NO direct reference to Core (transitive is fine)
    }

    /// <summary>Content.csproj ссылается только на Core.</summary>
    // [Fact]
    public void ContentProject_ReferencesOnlyCore()
    {
        // Arrange: load Content.csproj
        // Assert: exactly one ProjectReference → Core
    }

    // === INV-009: Project Reference Integrity ===

    /// <summary>Solution содержит все 5 проектов.</summary>
    // [Fact]
    public void Solution_ContainsAllProjects()
    {
        // Assert: Core, Runtime, UI, Content, Tests
    }

    /// <summary>Нет циклических зависимостей в графе проектов.</summary>
    // [Fact]
    public void ProjectGraph_HasNoCycles()
    {
        // Arrange: parse all .csproj files
        // Act: build dependency graph
        // Assert: topological sort succeeds (no cycles)
    }

    /// <summary>Directory.Build.props применяется ко всем проектам.</summary>
    // [Fact]
    public void DirectoryBuildProps_AppliesToAll()
    {
        // Assert: all projects inherit TargetFramework=net8.0
        // Assert: all projects inherit TreatWarningsAsErrors=true
    }

    // === Layer Responsibility ===

    /// <summary>Core assembly не содержит I/O классов.</summary>
    // [Fact]
    public void CoreAssembly_NoIOClasses()
    {
        // Arrange: scan Core types
        // Assert: no File, Stream, HttpClient usage
    }

    /// <summary>Content project не содержит классов с логикой.</summary>
    // [Fact]
    public void ContentProject_NoLogicClasses()
    {
        // Arrange: scan Content types
        // Assert: only data/schema types
    }
}
