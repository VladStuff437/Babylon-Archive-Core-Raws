# Чеклист внедрения — Сессия 003: Границы v1

## Core Layer
- [x] Создать `Contracts/` namespace в Core
- [x] Определить `IWorldStateReader` — read-only boundary
- [x] Определить `IWorldStateMutator` — command-based мутация
- [x] Определить `ICommand` — базовый контракт команды
- [x] Определить `ICommandDispatcher` — диспетчеризация команд
- [x] Определить `IGenerator` — seed-based генерация
- [x] Определить `IContentLoader` — загрузка/валидация контента
- [x] Определить `IViewStateProvider` — read-only проекция для UI
- [x] Определить `IInputHandler` — обработка ввода

## Runtime Layer
- [ ] Реализация контрактов — планируется в S004–S010

## UI Layer
- [ ] Адаптеры к boundary contracts — планируется в S010+

## Документация
- [x] ADR-003 с описанием boundaries
- [x] INV-008 в invariants.md
- [x] Boundary allowlist в ADR-003
- [x] Обновление pillar-module-mapping.md
- [x] Decision log обновлён

## Тесты
- [x] Шаблоны тестов для boundary violations
- [x] Тесты для interface segregation

## Конфигурация
- [x] Directory.Build.props — ревью boundary enforcement
