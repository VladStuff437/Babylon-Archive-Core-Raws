# Control Policy

## Профили управления
- Основной профиль: session-016-gameloop
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
