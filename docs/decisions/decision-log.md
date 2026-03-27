# Журнал решений — Babylon Archive Core

## Сессия 001 — Продуктовое видение

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-001 | Трёхслойная архитектура Core → Runtime → UI | Масштабируемость, изоляция доменной логики | ADR-001 |
| D-002 | WorldState как SSOT | Единая точка истины, предсказуемость | ADR-001 |
| D-003 | Seed-детерминизм для генерации | Воспроизводимость, тестируемость | ADR-001 |
| D-004 | Иерархическая адресация ArchiveAddress | Sector→Hall→Module→Shelf→Tome→Page — логичная навигация | ADR-001 |
| D-005 | .NET 8 как целевой фреймворк | LTS, производительность, кроссплатформенность | ADR-001 |
| D-006 | Система логирования с chat-UI | Требование Мастера (P0) — мониторинг процессов + заметки | — |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-001 | Выбор ECS-фреймворка | Ждём реализацию базовых систем | 011+ |
| P-002 | Формат save-файла | Зависит от финальной структуры WorldState | 008 |
| P-003 | Движок рендеринга | Определяется в блоке UI-фреймворков | 020+ |

### Требует эксперимента
| # | Гипотеза | Что проверить | Сессия |
|---|----------|---------------|--------|
| E-001 | Deterministic seed через System.Random | Property-based тесты на 10k seeds | 005 |
| E-002 | JSON vs MessagePack для save/load | Бенчмарки размера и скорости | 008 |

---

## Сессия 002 — Core Pillars

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-007 | 7 Core Pillars как неизменяемый фундамент | Предотвращение дрейфа дизайна на 150 сессиях | ADR-002 |
| D-008 | P1: Archive-Centric World | Архив — основная ось прогрессии, не декорация | ADR-002 |
| D-009 | P2: Deterministic Generation | Воспроизводимость, тестируемость, replay | ADR-002 |
| D-010 | P3: State Sovereignty (SSOT) | Предсказуемость, единая точка save/load | ADR-002 |
| D-011 | P4: Combat-Exploration Duality | Непрерывность мира, общий state | ADR-002 |
| D-012 | P5: Mission-Driven Progression | Миссии как основной контракт прогресса | ADR-002 |
| D-013 | P6: Layered Architecture Contract | Enforced через Directory.Build.props | ADR-001, ADR-002 |
| D-014 | P7: Content as Data | Баланс без перекомпиляции, автовалидация | ADR-002 |
| D-015 | INV-006: Combat-Exploration Continuity | Переход режимов без потери состояния | ADR-002 |
| D-016 | INV-007: Content as Validated Data | Контент валидируется до запуска | ADR-002 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-004 | Формат контентных схем (JSON Schema vs TypedDict) | Зависит от выбора runtime-валидатора | 020+ |
| P-005 | Конкретный PCG-алгоритм для seed | Зависит от бенчмарков в S005 | 005 |

### Требует эксперимента
| # | Гипотеза | Что проверить | Сессия |
|---|----------|---------------|--------|
| E-003 | Непрерывный combat (без transition screen) | Прототип mode-switch в GameLoop | 011+ |
| E-004 | YAML vs JSON для контентных файлов | Читаемость vs производительность парсинга | 020+ |

---

## Сессия 003 — Границы v1

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-017 | Boundary contracts через интерфейсы в Core.Contracts | Compile-time enforcement, ISP, тестируемость | ADR-003 |
| D-018 | Reader/Mutator разделение (IWorldStateReader vs IWorldStateMutator) | ISP: UI нужен только Reader, Runtime — оба | ADR-003 |
| D-019 | Command pattern для мутаций WorldState | Аудит, replay, undo-потенциал | ADR-003 |
| D-020 | IViewStateProvider для UI (read-only snapshots) | Pillar P3: State Sovereignty — UI не мутирует | ADR-003 |
| D-021 | IGenerator с DeriveChildSeed | INV-003: детерминизм иерархических seed'ов | ADR-003 |
| D-022 | INV-008: Boundary Contract Enforcement | Формализация cross-layer правил | ADR-003 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-006 | Roslyn analyzer для compile-time boundary check | Нужна стабильная кодовая база для правил | 015+ |

### Требует эксперимента
| # | Гипотеза | Что проверить | Сессия |
|---|----------|---------------|--------|
| E-005 | Command dispatch через очередь vs прямой вызов | Latency бенчмарк | 011+ |

---

## Сессия 004 — Слои архитектуры

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-023 | Отдельные .csproj на каждый слой | Compile-time enforcement зависимостей | ADR-004 |
| D-024 | Solution file объединяет все проекты | Стандарт .NET, IDE поддержка | ADR-004 |
| D-025 | Content → Core only (не Runtime) | Content — данные + схемы, не логика (P7) | ADR-004 |
| D-026 | Tests ссылается на все слои | Необходимо для integration + cross-layer тестов | ADR-004 |
| D-027 | INV-009: Project Reference Integrity | Ацикличный граф — блокирующий инвариант | ADR-004 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-007 | Solution folders для группировки проектов | Нхватает проектов для необходимости | 015+ |

---

## Сессия 005 — Системные инварианты

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-028 | Invariant validation framework в Core/Runtime | Runtime detection + тесты | ADR-005 |
| D-029 | IInvariantChecker как контракт в Core | Core определяет что, Runtime — как | ADR-005 |
| D-030 | InvariantResult как immutable value object | Предсказуемость, thread-safety | ADR-005 |
| D-031 | InvariantRegistry с register + check pattern | Расширяемость: новые INV добавляются без изменения registry | ADR-005 |
| D-032 | INV-010: Self-Testable Invariants | Каждый инвариант должен иметь checker | ADR-005 |

---

## Сессия 006 — ADR-контракт

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-033 | ADR lifecycle: Proposed → Accepted → Superseded/Deprecated | Правильные мутации статуса ADR | ADR-006 |
| D-034 | 7 обязательных секций в ADR | Единообразие, traceability | ADR-006 |
| D-035 | Traceability chain: ADR → INV → Code → Tests | Сквозная прослеживаемость от решения до теста | ADR-006 |
| D-036 | INV-011: ADR Traceability | Orphaned ADR = tech debt | ADR-006 |

---

## Сессия 007 — Структура репозитория

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-037 | Стандартная структура директорий (src/, tests/, docs/, tools/) | Масштабируемость, предсказуемость | ADR-007 |
| D-038 | Конвенции именования: PascalCase код, kebab-case docs | .NET стандарт + единообразие | ADR-007 |
| D-039 | Запрет .cs в root, .csproj только в src/tests | Чистота структуры | ADR-007 |
| D-040 | INV-012: Repository Layout Integrity | Автопроверка структуры | ADR-007 |

---

## Сессия 008 — Стандарты кода

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-041 | .editorconfig как единый enforcement | Кросс-IDE, build-time проверки | ADR-008 |
| D-042 | Allman braces + 4-space indent | .NET стандарт, читаемость | ADR-008 |
| D-043 | File-scoped namespaces обязательны | Меньше вложенности, C# 12 идиома | ADR-008 |
| D-044 | Запрет #region, dynamic, goto | Чистый, предсказуемый код | ADR-008 |
| D-045 | INV-013: Code Style Consistency | Единообразие на 150 сессий | ADR-008 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-008 | Roslyn analyzers для compile-time lint | Нужна стабильная кодовая база | 015+ |

---

## Сессия 009 — Базовый CI/CD

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-046 | GitHub Actions как базовый CI | Нативная интеграция с GitHub репозиторием | ADR-009 |
| D-047 | Pipeline stages: restore → build → test | Минимальный quality gate для каждого изменения | ADR-009 |
| D-048 | Триггеры push/PR на `main` + manual dispatch | Проверка merge-потока и ручных прогонов | ADR-009 |
| D-049 | INV-014: CI Pipeline Integrity | Формализация CI как архитектурного инварианта | ADR-009 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-009 | Coverage reporting + invariant checks в CI | Будет добавлено после расширения тестового контура | 010+ |

---

## Сессия 010 — Базовый тестовый контур

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-050 | Ввести baseline test contour для Block 01 | Единый quality gate перед переходом в Block 02 | ADR-010 |
| D-051 | Выделить smoke suite как быстрый слой проверок | Раннее обнаружение критических регрессий | ADR-010 |
| D-052 | Формализовать test categories: Smoke / Invariant / Session | Предсказуемая структура тестов при росте проекта | ADR-010 |
| D-053 | Принять testing-standards.md как стандарт процесса | Единообразие разработки и ревью | ADR-010 |
| D-054 | INV-015: Baseline Test Contour Integrity | Контур становится обязательным архитектурным контрактом | ADR-010 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-010 | Полноценная реализация assert-логики для всех шаблонных тестов | Приоритет смещён на создание каркаса Block 01 | 011+ |

---

## Сессия 011 — Схема данных игры v1

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-055 | Ввести Session011DataContract как baseline data model | Явный и типобезопасный контракт для runtime/content | ADR-011 |
| D-056 | Зафиксировать schema version 11 и JSON schema файл | Прослеживаемая валидация контента | ADR-011 |
| D-057 | Добавить serializer + migration step для версии 11 | Управляемая эволюция формата данных | ADR-011 |
| D-058 | INV-016: Stable Data Contract Baseline | Формализация критерия стабильности данных | ADR-011 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-011 | Полноценная JSON schema semantic validation в CI | Требует выделенного валидатора | 016+ |

---

## Сессия 012 — Схема сейвов v1

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-059 | Ввести Session012SaveContract для save payload | Стабильная структура сейва и диагностика | ADR-012 |
| D-060 | Повысить целевую версию схемы до 12 | Контролируемая эволюция save формата | ADR-012 |
| D-061 | Добавить migration 012 и integration template | Подготовка обратной совместимости | ADR-012 |
| D-062 | INV-017: Save Schema Backward Compatibility | Формальный инвариант совместимости сейвов | ADR-012 |

---

## Сессия 013 — Версионирование схем

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-063 | Ввести контракт управления версиями схем | Явная политика совместимости | ADR-013 |
| D-064 | Документировать schema-versioning и control fallback policy | Снижение операционных рисков | ADR-013 |
| D-065 | Добавить CI step проверки JSON-схем Session011-013 | Раннее обнаружение ошибок формата | ADR-013 |
| D-066 | INV-018: Schema Version Governance | Инвариант управления версиями | ADR-013 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-012 | Автоматическая дифф-проверка совместимости схем | Нужен отдельный schema diff tool | 017+ |

---

## Сессия 014 — Миграции данных

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-067 | Ввести Session014MigrationContract | Явный контракт migration pipeline | ADR-014 |
| D-068 | Добавить Migration_014 и serializer | Контролируемый апгрейд до schema v14 | ADR-014 |
| D-069 | Обновить schema-versioning policy до S014 | Прослеживаемость версий и шагов миграции | ADR-014 |
| D-070 | INV-019: Migration Pipeline Continuity | Формализация обязательности миграций | ADR-014 |

---

## Сессия 015 — Таксономия событий

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-071 | Ввести Session015EventTaxonomyContract | Нормализация event модели | ADR-015 |
| D-072 | Добавить EventTaxonomy и расширить SessionEvents | Стабильная классификация runtime-событий | ADR-015 |
| D-073 | Расширить EventBus реестром событий S014-S016 | Подготовка к event-driven контуру блока 02 | ADR-015 |
| D-074 | INV-020: Runtime Event Taxonomy Integrity | Формальный инвариант таксономии | ADR-015 |

---

## Сессия 016 — Каркас game loop

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-075 | Ввести Session016GameLoopContract | Явный контракт базового цикла | ADR-016 |
| D-076 | Добавить GameLoopScaffold (tick/pause/resume) | Минимальный управляемый runtime loop | ADR-016 |
| D-077 | Усилить валидацию CommandPipeline | Защита от некорректных командных состояний | ADR-016 |
| D-078 | INV-021: Game Loop Baseline Integrity | Формализация требований к loop baseline | ADR-016 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-013 | Детальные профили frame budget и fixed timestep | Требуется Runtime State Manager из S017 | 017+ |

---

## Сессия 017 — Runtime State Manager

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-079 | Ввести Session017RuntimeStateContract | Явная модель состояния runtime-менеджера | ADR-017 |
| D-080 | Добавить RuntimeStateManager с sync/transition/dirty-state | Управляемая синхронизация режима и тика | ADR-017 |
| D-081 | Добавить Migration_017 и Session017Serializer | Контролируемый апгрейд схемы до v17 | ADR-017 |
| D-082 | INV-022: Runtime State Manager Integrity | Формализация требований к runtime-state слою | ADR-017 |

---

## Сессия 018 — Командный контур действий

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-083 | Ввести Session018CommandContourContract | Явный контракт правил исполнения команд | ADR-018 |
| D-084 | Добавить CommandContourRegistry | Централизация validation-правил по mode/state | ADR-018 |
| D-085 | Расширить CommandPipeline contour validation | Ранний отказ до мутации WorldState | ADR-018 |
| D-086 | INV-023: Command Contour Validation Integrity | Формальный инвариант командного контура | ADR-018 |

---

## Сессия 019 — Action map ввода

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-087 | Ввести Session019ActionMapContract | Стандартизация профиля action map | ADR-019 |
| D-088 | Добавить ActionMapCatalog с fallback-путём | Надёжный резолв ввода при неполных профилях | ADR-019 |
| D-089 | Расширить CommandPipeline методом TryExecuteMappedInput | Связка action map и command contour в runtime | ADR-019 |
| D-090 | INV-024: Input Action Map Fallback Integrity | Формализация fallback-политики ввода | ADR-019 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-014 | Device-specific override слои action map | Требуется консолидация control profiles в S020 | 020 |

---

## Сессия 020 — Профили управления

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-091 | Ввести Session020ControlProfilesContract | Явный контракт active/fallback/profileChain | ADR-020 |
| D-092 | Добавить ControlProfileResolver в Runtime/Input | Централизация выбора профиля управления | ADR-020 |
| D-093 | Расширить CommandPipeline profile-chain execution | Прямая интеграция ввода с цепочкой профилей | ADR-020 |
| D-094 | INV-025: Control Profile Chain Integrity | Формализация консолидации control profiles v1 | ADR-020 |

### Отложено
| # | Решение | Причина | Планируемая сессия |
|---|---------|---------|-------------------|
| P-015 | Device-aware auto-selection профиля | Требуется runtime camera/input контур блока 03 | 021 |

---

## Сессия 024 — Атрибуты персонажа

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-095 | Расширить PlayerState атрибутами Strength/Agility/Intellect/Vitality | Базовый контур прогрессии персонажа | ADR-024 |
| D-096 | Ввести диапазонную валидацию атрибутов [1..100] | Предсказуемость и защита от некорректных значений | ADR-024 |
| D-097 | Добавить Session024 contract/schema/migration/serializer | Сквозная трассируемость и совместимость данных | ADR-024 |

---

## Сессия 025 — Система статусов

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-098 | Расширить StatusEffect полями category/stack/max-stack | Поддержка buff/debuff и stacking-механик | ADR-025 |
| D-099 | Добавить merge/remove API в StatusEffectManager | Явное управление жизненным циклом статусов | ADR-025 |
| D-100 | Добавить Session025 contract/schema/migration/serializer | Формализация статусного контура | ADR-025 |

---

## Сессия 026 — Ядро боевой системы

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-101 | Ввести CombatTurnContext в CombatSystem | Расширяемость боевого цикла без breaking changes | ADR-026 |
| D-102 | Добавить armor mitigation и critical multiplier | Минимально реалистичный core-расчёт боя | ADR-026 |
| D-103 | Сохранить legacy ResolveTurn(int,int) API | Обратная совместимость с предыдущими сессиями | ADR-026 |

---

## Сессия 027 — Tab и Esc в бою

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-104 | Добавить mission transition lock в CombatInputHandler | Исключение конфликтов таргетинга во время переходов | ADR-027 |
| D-105 | Зафиксировать поведение TabTarget/EscCancel под lock | Детерминированный input-поток для боевого контура | ADR-027 |
| D-106 | Добавить Session027 contract/schema/migration/serializer | Трассируемая эволюция контура боевого ввода | ADR-027 |

---

## Сессия 028 — Автоатака

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-107 | Перевести AutoAttackController на tick-модель | Управляемая и тестируемая частота ударов | ADR-028 |
| D-108 | Добавить mission transition suppression для автоатаки | Безопасность игрового цикла в фазах перехода | ADR-028 |
| D-109 | Добавить Session028 contract/schema/migration/serializer | Формализация состояния автоатаки | ADR-028 |

---

## Сессия 029 — Восприятие ИИ врагов

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-110 | Ввести awareness-aware детекцию целей в PerceptionSystem | Более реалистичная и настраиваемая перцепция | ADR-029 |
| D-111 | Добавить UpdateFromPerception в AIStateMachine | Явная связка перцепции и поведенческих переходов | ADR-029 |
| D-112 | Добавить Session029 contract/schema/migration/serializer | Сквозная трассируемость AI perception состояния | ADR-029 |

---

## Сессия 030 — State machine врагов

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-113 | Расширить AIState состояниями Alert/Investigate/ReturnToPost | Детализированный и предсказуемый поведенческий цикл врага | ADR-030 |
| D-114 | Добавить UpdateEnemyState(EnemyStateInput) в AIStateMachine | Формализация переходов по LOS/aggression/leash/health | ADR-030 |
| D-115 | Добавить Session030 contract/schema/migration/serializer | Трассируемая эволюция enemy AI state | ADR-030 |

---

## Сессия 031 — Формулы урона

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-116 | Ввести DamageCalculator с clamping граничных случаев | Стабильная и безопасная математическая модель урона | ADR-031 |
| D-117 | Добавить BalanceTableLoader и DropResolver | Перевод урона/лута в data-driven контур | ADR-031 |
| D-118 | Ввести MissionDefinition/MissionNode/TransitionEvaluator | Формализация миссионного runtime-контракта | ADR-031 |

---

## Сессия 032 — Базовый баланс v1

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-119 | Зафиксировать session-032.json как baseline profile | Единый baseline-множитель для боевой/экономической модели | ADR-032 |
| D-120 | Ввести WorldState с faction reputation и осями Moral/TechnoArcane | Явная state-модель эффектов миссий и баланса | ADR-032 |
| D-121 | Добавить Session032 contract/schema/migration/serializer | Сквозная трассируемость balance v1 | ADR-032 |

---

## Сессия 033 — Лут-система

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-122 | Добавить Session033 contract/schema/migration/serializer | Трассируемая эволюция лут-контура | ADR-033 |
| D-123 | Расширить DropResolver методами ResolveDropTier/ResolveDrop | Детализированный deterministic loot flow с luck bias | ADR-033 |
| D-124 | Зафиксировать session-033.json с rarity weights | Data-driven баланс редкостей без изменений кода | ADR-033 |

---

## Сессия 034 — Экономика v1

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-125 | Добавить Session034 contract/schema/migration/serializer | Формализация экономического профиля сессии | ADR-034 |
| D-126 | Расширить EconomyState инфляцией и faction modifiers | Управляемое ценообразование и ограничение диапазонов | ADR-034 |
| D-127 | Ввести ApplyFactionTransaction в WorldEconomySynchronizer | Явная связка world reputation и торговых транзакций | ADR-034 |

---

## Сессия 035 — Репутации фракций

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-128 | Добавить Session035 contract/schema/migration/serializer | Сквозная модель репутаций и осей мира | ADR-035 |
| D-129 | Расширить WorldState/WorldStateService API для осей и delta-репутаций | Единая точка изменения world-alignment состояния | ADR-035 |
| D-130 | Добавить CalculateDamageWithContext в DamageCalculator | Контролируемое влияние осей и репутации на боевые расчёты | ADR-035 |

---

## Сессия 036 — World Axis Consolidation

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-131 | Добавить Session036 contract/schema/migration/serializer | Формализация версии и структуры world-axis контракта | ADR-036 |
| D-132 | Расширить WorldState/WorldStateService snapshot/delta API для осей | Детерминированные и атомарные обновления осей мира в рантайме | ADR-036 |
| D-133 | Добавить Session036 runtime/mission/smoke tests | Подтверждение интеграции world-axis изменений с миссионным контуром | ADR-036 |

---

## Сессия 037 — MissionDefinition Contract Finalization

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-134 | Добавить Session037 contract/schema/migration/serializer | Формализация финальной версии MissionDefinition контракта | ADR-037 |
| D-135 | Усилить MissionDefinition.Validate правилами contractVersion/start-node | Предотвращение некорректных миссий до этапа исполнения | ADR-037 |
| D-136 | Добавить Session037 runtime/mission/smoke tests | Проверка совместимости контрактов и runtime-переходов | ADR-037 |

---

## Сессия 038 — MissionNode Contract Finalization

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-137 | Добавить Session038 contract/schema/migration/serializer | Формализация checkpoint/fallback полей узла миссии | ADR-038 |
| D-138 | Добавить fallback-семантику в MissionNode и deterministic fallback в TransitionEvaluator | Предсказуемый выбор перехода при отсутствии удовлетворённых условий | ADR-038 |
| D-139 | Добавить Session038 runtime/mission/smoke tests | Подтверждение стабильности fallback логики и миграции контракта | ADR-038 |

---

## Сессия 039 — Оценка переходов узлов

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-140 | Добавить Session039 contract/schema/migration/serializer | Формализация параметров scoring-модели переходов | ADR-039 |
| D-141 | Расширить TransitionEvaluator методами balance/scoring evaluation | Детализированный и настраиваемый выбор переходов при сохранении детерминизма | ADR-039 |
| D-142 | Добавить Session039 runtime/mission/smoke tests | Верификация tie-break, fallback и сериализации контракта | ADR-039 |

---

## Сессия 040 — Runtime миссии

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-143 | Добавить Session040 contract/schema/migration/serializer | Формализация runtime состояния миссии | ADR-040 |
| D-144 | Ввести MissionRuntimeEngine и MissionRuntimeState | Явный и тестируемый контур запуска/продвижения миссий | ADR-040 |
| D-145 | Добавить Session040 runtime/mission/smoke tests | Подтверждение deterministic progression и интеграции с баланс-профилем | ADR-040 |

---

## Сессия 041 — Reachability Validation

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-146 | Добавить Session041 contract/schema/migration/serializer | Формализация reachability-правил миссионного графа | ADR-041 |
| D-147 | Ввести ReachabilityValidator и pipeline MissionValidationRunner | Блокировка миссий с unreachable-узлами до runtime старта | ADR-041 |
| D-148 | Добавить MissionFactory fallback путь и Session041 tests | Гарантированный recovery-контур для невалидного контента | ADR-041 |

---

## Сессия 042 — Dead-End Safety

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-149 | Добавить Session042 contract/schema/migration/serializer | Формализация dead-end safety требований | ADR-042 |
| D-150 | Ввести DeadEndValidator для non-terminal узлов | Исключение тупиков без завершения/выхода в миссионном графе | ADR-042 |
| D-151 | Добавить MissionRuntimeSnapshot/Persistence и Session042 tests | Стабильное сохранение и восстановление mission runtime состояния | ADR-042 |

---

## Сессия 043 — Cycle Safety and Seeded Generation

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-152 | Добавить Session043 contract/schema/migration/serializer | Формализация cycle safety ограничений | ADR-043 |
| D-153 | Ввести CycleSafetyValidator с детекцией unsafe cycle | Предотвращение бесконечных замкнутых циклов без выхода | ADR-043 |
| D-154 | Добавить ArchiveAddress/ArchiveSeed/LevelGenerator и seed golden tests | Детерминированная генерация уровня по адресам Архива | ADR-043 |

---

## Сессия 044 — Fallback миссий

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-155 | Добавить Session044 contract/schema/migration/serializer | Формализация fallback-маршрута и reason-кодов | ADR-044 |
| D-156 | Нормализовать reason-коды в FallbackMissionProvider в детерминированном порядке | Идентичные наборы ошибок должны давать одинаковый fallback-результат | ADR-044 |
| D-157 | Добавить Session044 runtime/mission/generation tests | Подтверждение deterministic fallback поведения | ADR-044 |

---

## Сессия 045 — Save/load миссий

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-158 | Добавить Session045 contract/schema/migration/serializer | Формализация persistence metadata mission snapshot | ADR-045 |
| D-159 | Ввести SnapshotVersion и StateChecksum в MissionRuntimeSnapshot | Контроль совместимости и целостности restore-потока | ADR-045 |
| D-160 | Валидировать checksum при deserialize + добавить Session045 tests | Раннее выявление повреждённых snapshot-данных | ADR-045 |

---

## Сессия 046 — Модель ArchiveAddress

### Принято
| # | Решение | Обоснование | ADR |
|---|---------|-------------|-----|
| D-161 | Добавить Session046 contract/schema/migration/serializer | Формализация модели адресов Архива и seed-связки | ADR-046 |
| D-162 | Расширить ArchiveAddress навигацией (path/root/parent/next-page) | Явная и тестируемая модель адресной иерархии | ADR-046 |
| D-163 | Добавить ArchiveSeed.DeriveHierarchySeed и Session046 golden tests | Детерминированный replay generation по иерархии адреса | ADR-046 |
