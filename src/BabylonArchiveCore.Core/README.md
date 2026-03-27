# BabylonArchiveCore.Core

Доменный слой — чистые модели, контракты и состояние.

## Ответственность
- Доменные модели: WorldState, PlayerState, ArchiveAddress, MissionDefinition, MissionNode
- Контракты (интерфейсы) для Runtime
- Перечисления и value-objects
- Сериализуемое состояние

## Зависимости
- **Зависит от**: ничего (только BCL)
- **Запрещено**: ссылки на Runtime, UI, сторонние фреймворки

## Поддиректории (будут созданы в последующих сессиях)
```
Models/          — доменные модели
Contracts/       — интерфейсы
State/           — WorldState, PlayerState
Addressing/      — ArchiveAddress, иерархия
Missions/        — MissionDefinition, MissionNode
```
