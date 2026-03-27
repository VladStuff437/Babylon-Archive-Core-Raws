# Архитектурные инварианты — Babylon Archive Core

Этот документ фиксирует правила, нарушение которых является блокирующим дефектом.
Обновляется по мере принятия новых ADR.

---

## INV-001: WorldState — единственный источник правды (SSOT)

**Описание**: Всё мутабельное состояние мира хранится в `WorldState`. Никакой другой объект не дублирует и не кеширует изменяемое состояние мира.

**Проверка**:
- [ ] Grep по `public.*State` вне WorldState не содержит мутабельных полей мирового состояния
- [ ] Все изменения мира проходят через WorldState API

**Введён**: Сессия 001 / ADR-001

---

## INV-002: Однонаправленные зависимости слоёв

**Описание**: `Core ← Runtime ← UI`. Обратные зависимости запрещены:
- UI не ссылается на Core для мутации
- Runtime не ссылается на UI
- Core не ссылается ни на Runtime, ни на UI

**Проверка**:
- [ ] Directory.Build.props содержит правила запрета обратных ссылок
- [ ] CI проверяет граф зависимостей

**Введён**: Сессия 001 / ADR-001

---

## INV-003: Seed-детерминизм

**Описание**: Одинаковый seed → одинаковый результат генерации. Генератор не зависит от системного времени, порядка потоков или внешних данных.

**Проверка**:
- [ ] Property-based тест: `generate(seed) == generate(seed)` для N итераций
- [ ] Replay-тесты воспроизводят записанные сессии

**Введён**: Сессия 001 (будет реализован в Сессии 005)

---

## INV-004: Иерархия ArchiveAddress

**Описание**: Адрес в Архиве строго иерархичен: `Sector → Hall → Module → Shelf → Tome → Page`. Каждый уровень детерминированно вычисляется из родительского seed.

**Проверка**:
- [ ] ArchiveAddress.Parse / ToString round-trip
- [ ] Навигация вверх/вниз по иерархии корректна

**Введён**: Сессия 001 (будет реализован в Сессии 006)

---

## INV-005: Миссии валидируются до запуска

**Описание**: Каждая MissionDefinition проходит валидацию перед запуском: достижимость, отсутствие dead-end без fallback, безопасность циклов.

**Проверка**:
- [ ] MissionValidator.Validate() вызывается перед MissionRunner.Start()
- [ ] Невалидная миссия не запускается

**Введён**: Сессия 001 (будет реализован в Сессии 007)

---

## INV-006: Combat-Exploration Continuity

**Описание**: Переход между режимами exploration и combat происходит без потери состояния. Оба режима работают с единым WorldState. Результат боя влияет на состояние Архива.

**Проверка**:
- [ ] WorldState сохраняет контекст при transition exploration→combat→exploration
- [ ] Нет дублирования состояния между режимами
- [ ] Combat outcome записывается в WorldState и отражается в Архиве

**Введён**: Сессия 002 / ADR-002 (Pillar P4)

---

## INV-007: Content as Validated Data

**Описание**: Весь игровой контент (миссии, баланс, шаблоны Архива) описывается как данные (JSON/YAML). Контент валидируется автоматически до запуска и до merge. Баланс изменяется без перекомпиляции.

**Проверка**:
- [ ] Контентные файлы имеют schema-валидацию
- [ ] CI запускает ContentValidator перед merge
- [ ] Изменение баланса не требует пересборки кода

**Введён**: Сессия 002 / ADR-002 (Pillar P7)

---

## INV-008: Boundary Contract Enforcement

**Описание**: Все cross-layer взаимодействия проходят через контрактные интерфейсы из `Core.Contracts`. Прямые зависимости на конкретные классы другого слоя запрещены. Reader/Mutator разделены (ISP). UI получает только read-only проекции.

**Проверка**:
- [ ] Все public boundary interfaces в `BabylonArchiveCore.Core.Contracts` namespace
- [ ] Runtime реализует Core contracts, не наоборот
- [ ] UI использует только IViewStateProvider и IInputHandler
- [ ] Никакой слой не обходит контрактные интерфейсы

**Введён**: Сессия 003 / ADR-003 (Pillar P6, INV-002)

---

## INV-009: Project Reference Integrity

**Описание**: Граф зависимостей проектов строго ацикличен. Core не ссылается ни на что, Runtime — только на Core, UI — на Runtime, Content — на Core. Нарушение — ошибка сборки.

**Проверка**:
- [ ] Core.csproj не содержит ProjectReference
- [ ] Runtime.csproj → только Core
- [ ] UI.csproj → только Runtime
- [ ] Content.csproj → только Core
- [ ] Топологическая сортировка успешна (нет циклов)

**Введён**: Сессия 004 / ADR-004

---

## INV-010: Self-Testable Invariants

**Описание**: Каждый архитектурный инвариант должен иметь соответствующий `IInvariantChecker`, зарегистрированный в `IInvariantRegistry`. Инварианты без автоматической проверки — tech debt.

**Проверка**:
- [ ] Все INV-xxx имеют соответствующий IInvariantChecker
- [ ] CheckAll() в InvariantRegistry возвращает результаты для всех зарегистрированных
- [ ] CI запускает invariant checks при каждом build

**Введён**: Сессия 005 / ADR-005

---

## INV-011: ADR Traceability

**Описание**: Каждый ADR должен быть привязан к минимум одному инварианту. Каждый инвариант должен ссылаться на ADR-источник. ADR INDEX.md содержит записи для всех ADR-файлов. Orphaned ADR (без INV-связи) — tech debt.

**Проверка**:
- [ ] Все ADR-*.md содержат секцию "Связи" с INV-ссылками
- [ ] INDEX.md синхронизирован с файловой системой docs/adr/
- [ ] Каждый ADR имеет валидный lifecycle статус

**Введён**: Сессия 006 / ADR-006

---

## INV-012: Repository Layout Integrity

**Описание**: Структура репозитория соответствует стандарту ADR-007. Обязательные директории (src/, tests/, docs/, tools/, MasterPlan/) существуют. .cs файлы не в корне. .csproj только в src/ и tests/. Каждый проект имеет README.md.

**Проверка**:
- [ ] Обязательные корневые директории существуют
- [ ] Нет .cs в root
- [ ] Все .csproj внутри src/ или tests/
- [ ] docs/ содержит стандартные поддиректории

**Введён**: Сессия 007 / ADR-007

---

## INV-013: Code Style Consistency

**Описание**: Весь код в репозитории соответствует стандартам ADR-008. .editorconfig является enforcement-инструментом. File-scoped namespaces обязательны. Naming conventions enforced. Нет #region, dynamic, goto.

**Проверка**:
- [ ] .editorconfig существует и содержит root=true
- [ ] Все .cs используют file-scoped namespaces
- [ ] Нет #region в коде
- [ ] Public API имеет XML-docs

**Введён**: Сессия 008 / ADR-008

---

## INV-014: CI Pipeline Integrity

**Описание**: Репозиторий должен иметь рабочий CI pipeline в GitHub Actions, который запускается на push/PR в `main`, выполняет `restore`, `build` и `test` для `BabylonArchiveCore.sln` на .NET 8.

**Проверка**:
- [ ] `.github/workflows/ci.yml` существует
- [ ] Workflow содержит шаги `dotnet restore`, `dotnet build`, `dotnet test`
- [ ] Триггеры `push` и `pull_request` на ветку `main` присутствуют

**Введён**: Сессия 009 / ADR-009

---

## INV-015: Baseline Test Contour Integrity

**Описание**: В проекте должен существовать базовый тестовый контур: сессионные тесты `Session001..Session010`, smoke-набор для Block 01 и документированные стандарты тестирования. Отсутствие этих артефактов блокирует развитие следующих блоков.

**Проверка**:
- [ ] Существуют `Session001..Session010` в `tests/BabylonArchiveCore.Tests/`
- [ ] Существует smoke-набор `tests/BabylonArchiveCore.Tests/Smoke/Block01SmokeTests.cs`
- [ ] Существует `docs/standards/testing-standards.md`
- [ ] CI workflow запускает `dotnet test`

**Введён**: Сессия 010 / ADR-010

---

## INV-016: Stable Data Contract Baseline

**Описание**: Для сессии 011 должен существовать стабильный контракт игровых данных и соответствующая JSON-схема. Runtime сериализация обязана поддерживать этот контракт без потери обязательных полей.

**Проверка**:
- [ ] Существует `Session011DataContract` в Core.Contracts
- [ ] Существует `session-011.schema.json` в Content/Schemas
- [ ] Существует runtime serializer и migration step для версии 11

**Введён**: Сессия 011 / ADR-011

---

## INV-017: Save Schema Backward Compatibility

**Описание**: Save-схема сессии 012 должна быть обратно совместима через migration step. Любой legacy save должен быть обновляем до текущей версии схемы.

**Проверка**:
- [ ] Существует `Session012SaveContract` и `session-012.schema.json`
- [ ] Существует `Migration_012` с повышением `schemaVersion`
- [ ] Integration шаблон save/load присутствует

**Введён**: Сессия 012 / ADR-012

---

## INV-018: Schema Version Governance

**Описание**: Версии схем должны управляться явным контрактом (current/minimum compatible/channel/migration policy) и валидироваться в CI на уровне наличия и корректного JSON формата.

**Проверка**:
- [ ] Существует `Session013SchemaVersionContract` и `session-013.schema.json`
- [ ] Документ `docs/data/schema-versioning.md` актуален
- [ ] CI содержит шаг валидации session schema файлов

**Введён**: Сессия 013 / ADR-013

---

## INV-019: Migration Pipeline Continuity

**Описание**: Каждый новый schema version increment обязан сопровождаться явным migration step в Runtime/Migrations и контрактом миграции в Core.Contracts.

**Проверка**:
- [ ] Существует `Session014MigrationContract`
- [ ] Существует `Migration_014`
- [ ] Integration тест migration flow присутствует

**Введён**: Сессия 014 / ADR-014

---

## INV-020: Runtime Event Taxonomy Integrity

**Описание**: Runtime-события должны соответствовать таксономии (domain/category/severity) и иметь стабильные именованные константы в SessionEvents.

**Проверка**:
- [ ] Существует `Session015EventTaxonomyContract`
- [ ] Существует `EventTaxonomy` с доменами и категориями
- [ ] EventBus регистрирует события S014-S016

**Введён**: Сессия 015 / ADR-015

---

## INV-021: Game Loop Baseline Integrity

**Описание**: Runtime должен иметь минимальный управляемый game loop scaffold с инкрементом тика, поддержкой pause/resume и безопасной обработкой команд.

**Проверка**:
- [ ] Существует `Session016GameLoopContract`
- [ ] Существует `GameLoopScaffold`
- [ ] CommandPipeline валидирует некорректные состояния до исполнения команды

**Введён**: Сессия 016 / ADR-016

---

## INV-022: Runtime State Manager Integrity

**Описание**: Runtime должен иметь единый менеджер состояния, синхронизирующий режим и тик с WorldStateReader, а также фиксирующий dirty-state после переходов.

**Проверка**:
- [ ] Существует `Session017RuntimeStateContract`
- [ ] Существует `RuntimeStateManager`
- [ ] RuntimeStateManager поддерживает sync/transition/dirty-state

**Введён**: Сессия 017 / ADR-017

---

## INV-023: Command Contour Validation Integrity

**Описание**: Все команды, для которых задан command contour, обязаны проходить mode-aware и state-aware валидацию до мутации WorldState.

**Проверка**:
- [ ] Существует `Session018CommandContourContract`
- [ ] Существует `CommandContourRegistry`
- [ ] `CommandPipeline` выполняет contour validation до `ApplyCommand`

**Введён**: Сессия 018 / ADR-018

---

## INV-024: Input Action Map Fallback Integrity

**Описание**: Разрешение действий ввода должно выполняться через action map профиль с fallback-маршрутом, исключая жёстко кодированные бинды в pipeline.

**Проверка**:
- [ ] Существует `Session019ActionMapContract`
- [ ] Существует `ActionMapCatalog` с fallback-резолвом
- [ ] `CommandPipeline` поддерживает mapped-input execution

**Введён**: Сессия 019 / ADR-019

---

## INV-025: Control Profile Chain Integrity

**Описание**: Выбор профиля управления обязан выполняться через консолидацию active/fallback/profileChain в runtime-слое с предсказуемым fallback-маршрутом.

**Проверка**:
- [ ] Существует `Session020ControlProfilesContract`
- [ ] Существует `ControlProfileResolver`
- [ ] `CommandPipeline` поддерживает profile-chain execution

**Введён**: Сессия 020 / ADR-020
