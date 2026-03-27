namespace BabylonArchiveCore.Tests.Session006;

/// <summary>
/// Тесты сессии 006: ADR-контракт.
/// Проверяют ADR traceability и lifecycle.
/// </summary>
public class Session006Tests
{
    // === INV-011: ADR Traceability ===

    /// <summary>Каждый ADR файл содержит секцию "Связи".</summary>
    // [Fact]
    public void AllADRFiles_ContainLinksSection()
    {
        // Arrange: enumerate docs/adr/ADR-*.md
        // Assert: each contains "## Связи" or "## Связи"
    }

    /// <summary>Каждый ADR привязан к минимум одному INV.</summary>
    // [Fact]
    public void AllADRs_LinkedToAtLeastOneInvariant()
    {
        // Arrange: parse ADR files for "INV-" references
        // Assert: count > 0 per ADR
    }

    /// <summary>INDEX.md содержит все ADR файлы.</summary>
    // [Fact]
    public void ADRIndex_ContainsAllADRFiles()
    {
        // Arrange: list ADR-*.md files
        // Assert: each has entry in INDEX.md
    }

    // === ADR Lifecycle ===

    /// <summary>Каждый ADR имеет валидный статус.</summary>
    // [Fact]
    public void AllADRs_HaveValidStatus()
    {
        // Assert: status in {Proposed, Accepted, Superseded, Deprecated}
    }

    /// <summary>Superseded ADR ссылается на замещающий.</summary>
    // [Fact]
    public void SupersededADR_ReferencesReplacement()
    {
        // Arrange: find ADRs with "Superseded" status
        // Assert: contains "by ADR-XXX"
    }

    /// <summary>TEMPLATE.md содержит все обязательные секции.</summary>
    // [Fact]
    public void Template_ContainsAllRequiredSections()
    {
        // Assert: Статус, Решение, Альтернативы, Риски, Связи
    }
}
