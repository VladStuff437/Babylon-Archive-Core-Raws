# Pillar-to-Module Mapping — Сессия 002

Каждый Core Pillar требует реализации в конкретных модулях. Этот документ фиксирует границы ответственности.

## Маппинг Pillar → Модули

| Pillar | Core | Runtime | UI | Content | Tests |
|--------|------|---------|----|---------|-------|
| P1: Archive-Centric World | ArchiveAddress, Models | Generation | Archive browser, Map | Archive templates | Address round-trip, hierarchy navigation |
| P2: Deterministic Generation | — (seed value в ArchiveAddress) | Generation (seed engine) | — | — | Property-based seed tests, replay |
| P3: State Sovereignty | WorldState, PlayerState | Commands, GameLoop | Read-only bindings | — | SSOT invariant tests |
| P4: Combat-Exploration | CombatState (часть WorldState) | Combat/, GameLoop (mode switch) | Combat HUD, Exploration HUD | — | Transition state tests |
| P5: Mission-Driven | MissionDefinition, MissionNode | MissionRunner, MissionValidator | Mission UI, Journal | Mission definitions (YAML) | Reachability, dead-end, cycle tests |
| P6: Layered Architecture | Contracts/ | Implementations/ | Adapters/ | — | Dependency graph tests |
| P7: Content as Data | Schema types | ContentLoader, ContentValidator | — | *.json, *.yaml, schemas | Schema validation tests |

## Boundaries (запрещённые связи)
- UI **не** создаёт ArchiveAddress напрямую → через Runtime API
- Content **не** содержит логику → только данные и схемы
- Combat state **не** живёт отдельно от WorldState
- Generation **не** использует DateTime/Thread → только seed + PCG

## Boundary Contracts (Сессия 003, ADR-003)

### Core → Runtime Contracts
| Интерфейс | Pillar | Назначение |
|-----------|--------|------------|
| `IWorldStateReader` | P3, P6 | Read-only доступ к WorldState |
| `IWorldStateMutator` | P3, P6 | Command-based мутация |
| `ICommand` | P3 | Базовый контракт команды |
| `ICommandDispatcher` | P3, P6 | Диспетчеризация команд |
| `IGenerator` | P2, P6 | Seed-based генерация |
| `IContentLoader` | P7, P6 | Загрузка/валидация контента |

### Runtime → UI Contracts
| Интерфейс | Pillar | Назначение |
|-----------|--------|------------|
| `IViewStateProvider` | P3, P6 | Read-only проекция для UI |
| `IInputHandler` | P6 | Обработка ввода |

Все контракты: `src/BabylonArchiveCore.Core/Contracts/`
