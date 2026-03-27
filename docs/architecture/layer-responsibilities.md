# Ответственности слоёв — Babylon Archive Core

## Core (BabylonArchiveCore.Core)
**Зависимости**: нет
**Ответственность**:
- Доменные модели: WorldState, PlayerState, ArchiveAddress, MissionDefinition
- Контрактные интерфейсы: `Contracts/` namespace (ADR-003)
- Enums: LogLevel, GameMode, AddressLevel
- DTO / Value Objects: LogEntry, immutable records
- Нет бизнес-логики, нет I/O, нет side-effects

## Runtime (BabylonArchiveCore.Runtime)
**Зависимости**: Core
**Ответственность**:
- GameLoop: тиковый цикл обновления мира
- Реализация Core contracts: WorldStateMutator, CommandDispatcher, Generator
- Генерация: seed-based алгоритмы, PCG
- Combat: боевая система, AI
- Save/Load: сериализация WorldState
- Logging: LogService, LogFileWriter

## UI (BabylonArchiveCore.UI)
**Зависимости**: Runtime (транзитивно Core)
**Ответственность**:
- HUD: информационные панели, статусы
- Рендеринг: карта Архива, боевой экран
- Input: обработка ввода игрока → IInputHandler
- Адаптеры: чтение через IViewStateProvider
- Logging UI: LogChatWindow, ConsoleLogRenderer
- **Никогда** не мутирует WorldState напрямую

## Content (BabylonArchiveCore.Content)
**Зависимости**: Core
**Ответственность**:
- JSON/YAML файлы данных
- Schema definitions (используя Core-типы)
- Шаблоны миссий, баланс, описания
- **Никогда** не содержит логику — только данные

## Tests (BabylonArchiveCore.Tests)
**Зависимости**: Core, Runtime, UI, Content
**Ответственность**:
- Unit тесты для каждого слоя
- Integration тесты для cross-layer взаимодействий
- Property-based тесты для инвариантов
- Regression тесты
