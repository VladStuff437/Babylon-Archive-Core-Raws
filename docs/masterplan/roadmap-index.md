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
| 017 | Runtime State Manager | Управление состоянием рантайма | Артефакты S011–016 | Runtime state manager + tests | ⏳ |
| 018 | Командный контур действий | Расширение command pipeline | Артефакты S011–017 | Command contour + tests | ⏳ |
| 019 | Action map ввода | Карта действий и профили | Артефакты S011–018 | Action map + tests | ⏳ |
| 020 | Профили управления | Консолидация control profiles | Все S011–019 | Control profiles v1 + tests | ⏳ |

## Блоки 03–15
> Детализация по мере продвижения. См. MasterPlan.md для полного списка 150 сессий.
