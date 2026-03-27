namespace BabylonArchiveCore.Tests.Session007;

/// <summary>
/// Тесты сессии 007: Структура репозитория.
/// Проверяют соответствие файловой структуры стандарту ADR-007.
/// </summary>
public class Session007Tests
{
    // === INV-012: Repository Layout Integrity ===

    /// <summary>Обязательные корневые директории существуют.</summary>
    // [Fact]
    public void RequiredRootDirectories_Exist()
    {
        // Assert: src/, tests/, docs/, tools/, MasterPlan/ exist
    }

    /// <summary>Нет .cs файлов в корне репозитория.</summary>
    // [Fact]
    public void NoCSharpFiles_InRoot()
    {
        // Assert: no *.cs in repository root
    }

    /// <summary>.csproj файлы только в src/ и tests/.</summary>
    // [Fact]
    public void ProjectFiles_OnlyInSrcAndTests()
    {
        // Assert: all *.csproj under src/ or tests/
    }

    /// <summary>Каждый проект имеет README.md.</summary>
    // [Fact]
    public void EachProject_HasReadme()
    {
        // Assert: src/*/README.md exists for each project
    }

    /// <summary>docs/ содержит стандартные поддиректории.</summary>
    // [Fact]
    public void DocsDirectory_HasStandardSubfolders()
    {
        // Assert: adr/, architecture/, checklists/, decisions/, masterplan/, reports/, standards/
    }

    /// <summary>Тестовые папки именуются Session{NNN}.</summary>
    // [Fact]
    public void TestFolders_FollowNamingConvention()
    {
        // Assert: all subdirs in tests project match "Session\d{3}"
    }

    /// <summary>.gitignore существует и не пуст.</summary>
    // [Fact]
    public void GitIgnore_ExistsAndNotEmpty()
    {
        // Assert: .gitignore exists and has content
    }
}
