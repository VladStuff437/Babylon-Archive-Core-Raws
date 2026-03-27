# Roadmap Index — Babylon Archive Core

## Блок 01. Стратегия и архитектурный фундамент (001–010)

| Сессия | Название | Цель | Входные артефакты | Критерии завершения | Статус |
|--------|----------|------|-------------------|---------------------|--------|
| 001 | Продуктовое видение | Сформировать управляемый фундамент разработки и зафиксировать правила | MasterPlan.md | s001.md, ADR-001, invariants.md, Directory.Build.props, decision-log, closure report, logging system | ✅ Завершена |
| 002 | Core pillars | Зафиксировать правила, не меняющиеся в следующих фазах | Артефакты S001 | s002.md, ADR-002, обновлённые invariants, тесты | ✅ Завершена |
| 003 | Границы v1 | Границы между слоями (boundary contracts) | Артефакты S001–002 | s003.md, ADR-003, 8 boundary interfaces, INV-008 | ✅ Завершена |
| 004 | Слои архитектуры | Формальная структура проектов (.csproj, .sln) | Артефакты S001–003 | s004.md, ADR-004, 5 .csproj, .sln, INV-009, layer-responsibilities | ✅ Завершена |
| 005 | Системные инварианты | Кодификация инвариантов (runtime validation) | Артефакты S001–004 | s005.md, ADR-005, invariant framework (7 файлов кода), INV-010 | ✅ Завершена |
| 006 | ADR-контракт | Формализация ADR-процесса | ADR-001..005 | ADR-006, TEMPLATE.md, INDEX.md, INV-011 | ✅ Завершена |
| 007 | Структура репозитория | Стандарты организации репозитория | Артефакты S001–006 | s007.md, ADR-007, repo conventions, .gitignore | ✅ Завершена |
| 008 | Стандарты кода | Код-стайл, .editorconfig, naming | Артефакты S001–007 | s008.md, ADR-008, .editorconfig, coding-standards.md | ✅ Завершена |
| 009 | Базовый CI/CD | GitHub Actions pipeline | Артефакты S001–008 | s009.md, ADR-009, CI workflow | ✅ Завершена |
| 010 | Базовый тестовый контур | Тест-фреймворк, Block 01 checkpoint | Все S001–009 | s010.md, ADR-010, smoke тесты, Block 01 milestone | ✅ Завершена |

## Блок 02. Доменные схемы, сейвы и ввод (011–020)

| Сессия | Название | Цель | Входные артефакты | Критерии завершения | Статус |
|--------|----------|------|-------------------|---------------------|--------|
| 011 | Схема данных игры v1 | Контракты данных и базовый runtime | Артефакты S001–010 | Session011 contracts/schema/serializer/migration/tests | ✅ Завершена |
| 012 | Схема сейвов v1 | Стабилизация save контракта | Артефакты S011 | Session012 contracts/schema/serializer/migration/tests | ✅ Завершена |
| 013 | Версионирование схем | Правила совместимости и версии | Артефакты S011–012 | Session013 contracts/schema/serializer/migration/tests | ✅ Завершена |
| 014 | Миграции данных | Консолидация migration pipeline | Артефакты S011–013 | Migration_014 + tests | ✅ Завершена |
| 015 | Таксономия событий | Нормализация runtime event model | Артефакты S011–014 | Event taxonomy + tests | ✅ Завершена |
| 016 | Каркас game loop | Базовый управляемый цикл runtime | Артефакты S011–015 | GameLoop scaffold + validation + tests | ✅ Завершена |
| 017 | Runtime State Manager | Управление состоянием рантайма | Артефакты S011–016 | Runtime state manager + tests | ✅ Завершена |
| 018 | Командный контур действий | Расширение command pipeline | Артефакты S011–017 | Command contour + tests | ✅ Завершена |
| 019 | Action map ввода | Карта действий и профили | Артефакты S011–018 | Action map + tests | ✅ Завершена |
| 020 | Профили управления | Консолидация control profiles | Все S011–019 | Control profiles v1 + tests | ✅ Завершена |

## Блоки 03–15
> Детализация по мере продвижения. См. MasterPlan.md для полного списка 150 сессий.

## Блок 03. Геймплейный контур (021–029)

| Сессия | Название | Цель | Входные артефакты | Критерии завершения | Статус |
|--------|----------|------|-------------------|---------------------|--------|
| 021 | Camera System Foundation | Режимы камеры и базовый runtime camera state | Артефакты S011–020 | Session021 contract/schema/runtime/tests | ✅ Завершена |
| 022 | Система взаимодействий | Interactions/inventory/player/combat/ai baseline | Артефакты S011–021 | Session022 contour + smoke tests | ✅ Завершена |
| 023 | Инвентарь v1 | Консолидация инвентаря и round-trip | Артефакты S011–022 | Session023 contract/schema/migration/tests | ✅ Завершена |
| 024 | Атрибуты персонажа | Расширение PlayerState атрибутами | Артефакты S011–023 | Session024 attributes + smoke tests | ✅ Завершена |
| 025 | Система статусов | Stackable status lifecycle | Артефакты S011–024 | Session025 status + smoke tests | ✅ Завершена |
| 026 | Ядро боевой системы | Контекстный боевой расчёт | Артефакты S011–025 | Session026 combat core + smoke tests | ✅ Завершена |
| 027 | Tab и Esc в бою | Безопасный контур таргетинга в переходах миссий | Артефакты S011–026 | Session027 combat input + smoke tests | ✅ Завершена |
| 028 | Автоатака | Тиковая автоатака с безопасной блокировкой | Артефакты S011–027 | Session028 auto-attack + smoke tests | ✅ Завершена |
| 029 | Восприятие ИИ врагов | Детекция/awareness/primary target для AI | Артефакты S011–028 | Session029 perception + smoke tests | ✅ Завершена |

## Блок 04. Боевые расчеты, баланс и миссионные контракты (030–040)

| Сессия | Название | Цель | Входные артефакты | Критерии завершения | Статус |
|--------|----------|------|-------------------|---------------------|--------|
| 030 | State machine врагов | Расширение поведения врагов и переходов AI | Артефакты S011–029 | Session030 enemy state machine + smoke tests | ✅ Завершена |
| 031 | Формулы урона | Формализация damage/loot/economy/mission runtime контуров | Артефакты S011–030 | Session031 formulas + mission integration tests | ✅ Завершена |
| 032 | Базовый баланс v1 | Фиксация baseline balance profile и синхронизация с runtime | Артефакты S011–031 | Session032 balance + mission integration tests | ✅ Завершена |
| 033 | Лут-система | Формализация редкостей, luck bias и deterministic drop resolution | Артефакты S011–032 | Session033 loot contracts/runtime + mission integration tests | ✅ Завершена |
| 034 | Экономика v1 | Инфляция, faction pricing и экономические транзакции runtime | Артефакты S011–033 | Session034 economy contracts/runtime + mission integration tests | ✅ Завершена |
| 035 | Репутации фракций | Оси мира и репутации как фактор runtime-расчётов | Артефакты S011–034 | Session035 faction/world-state contracts/runtime + mission integration tests | ✅ Завершена |
| 036 | World Axis Consolidation | Консолидация осей мира и snapshot/delta API для рантайма | Артефакты S011–035 | Session036 contracts/schema/runtime/tests + mission integration tests | ✅ Завершена |
| 037 | MissionDefinition Contract Finalization | Финализация контракта миссий и валидации start-node/переходов | Артефакты S011–036 | Session037 contracts/schema/runtime/tests + mission integration tests | ✅ Завершена |
| 038 | MissionNode Contract Finalization | Контракт узлов миссий: checkpoint/fallback и детерминированный резолв переходов | Артефакты S011–037 | Session038 contracts/schema/runtime/tests + mission integration tests | ✅ Завершена |
| 039 | Оценка переходов узлов | Детализация scoring-модели переходов миссий и баланс-управляемый выбор | Артефакты S011–038 | Session039 contracts/schema/runtime/tests + mission integration tests | ✅ Завершена |
| 040 | Runtime миссии | Формализация выполнения миссии: state/orchestration/advance semantics | Артефакты S011–039 | Session040 contracts/schema/runtime/tests + mission integration tests | ✅ Завершена |

## Блок 05. Валидация графа миссий и seed-генерация (041–050)

| Сессия | Название | Цель | Входные артефакты | Критерии завершения | Статус |
|--------|----------|------|-------------------|---------------------|--------|
| 041 | Reachability Validation | Ввести обязательную проверку достижимости узлов миссии до runtime старта | Артефакты S011–040 | Session041 contracts/schema/runtime/tests + fallback integration | ✅ Завершена |
| 042 | Dead-End Safety | Исключить non-terminal dead-end ветки и добавить snapshot persistence mission runtime | Артефакты S011–041 | Session042 contracts/schema/runtime/tests + persistence validation | ✅ Завершена |
| 043 | Cycle Safety and Seeded Generation | Добавить безопасность циклов и детерминированную генерацию уровней по ArchiveAddress/seed | Артефакты S011–042 | Session043 contracts/schema/runtime/generation tests + seed golden tests | ✅ Завершена |
| 044 | Fallback миссий | Детерминированный fallback-маршрут для невалидных миссий и фиксация reason-кодов | Артефакты S011–043 | Session044 contracts/schema/runtime/mission/generation tests | ✅ Завершена |
| 045 | Save/load миссий | Интеграция checksum-валидации и версии snapshot в mission persistence | Артефакты S011–044 | Session045 contracts/schema/runtime/mission/generation tests | ✅ Завершена |
| 046 | Модель ArchiveAddress | Навигационная модель адресов и иерархический seed-контур Архива | Артефакты S011–045 | Session046 contracts/schema/runtime/mission/generation tests | ✅ Завершена |
