# Чеклист внедрения — Сессия 004: Слои архитектуры

## Проекты
- [x] Создать `BabylonArchiveCore.Core.csproj` (без зависимостей)
- [x] Создать `BabylonArchiveCore.Runtime.csproj` (→ Core)
- [x] Создать `BabylonArchiveCore.UI.csproj` (→ Runtime)
- [x] Создать `BabylonArchiveCore.Content.csproj` (→ Core)
- [x] Создать `BabylonArchiveCore.Tests.csproj` (→ All)
- [x] Создать `BabylonArchiveCore.sln`

## Зависимости (INV-002 enforcement)
- [x] Core не ссылается ни на что
- [x] Runtime ссылается только на Core
- [x] UI ссылается на Runtime (транзитивно Core)
- [x] Content ссылается только на Core
- [x] Нет циклических зависимостей

## Документация
- [x] ADR-004 — project structure
- [x] `docs/architecture/layer-responsibilities.md` — ответственности
- [x] INV-009 в invariants.md
- [x] Decision log обновлён

## Тесты
- [x] Шаблоны тестов для project dependency validation
