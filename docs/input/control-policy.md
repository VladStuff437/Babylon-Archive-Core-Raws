# Control Policy

## Профили управления
- Основной профиль: session-020-controls
- Fallback профиль: fallback

## Правила fallback
- Если активный профиль не найден, система обязана переключиться на fallback.
- Fallback содержит минимальный набор bind-команд для безопасного управления.

## Эволюция профилей
- Session011: archive и mission bindings
- Session012: quick save/load bindings
- Session013: schema/version debug toggle
- Session014: migration controls
- Session015: event console controls
- Session016: game loop pause/step controls
- Session017: runtime state snapshot/switch controls
- Session018: command contour confirm/cancel controls
- Session019: action map access and quick slots
- Session020: consolidated profile chain controls
