# Чеклист внедрения — Сессия 001

## Порядок изменений по слоям

### Фаза A: Документация и планирование
- [x] 1. Создать `docs/masterplan/s001.md`
- [x] 2. Обновить `docs/masterplan/roadmap-index.md`
- [x] 3. Создать `docs/adr/ADR-001.md`
- [x] 4. Создать `docs/architecture/invariants.md`

### Фаза B: Структура проекта
- [x] 5. Создать папки `src/BabylonArchiveCore.Core/`
- [x] 5. Создать папки `src/BabylonArchiveCore.Runtime/`
- [x] 5. Создать папки `src/BabylonArchiveCore.UI/`
- [x] 5. Создать папки `src/BabylonArchiveCore.Content/`
- [x] 5. Создать папки `tests/BabylonArchiveCore.Tests/`
- [x] 5. Создать папки `tools/`

### Фаза C: Конфигурация и ограничения
- [x] 7. Шаблоны тестов Session001
- [x] 8. `Directory.Build.props` с правилами зависимостей

### Фаза D: Фиксация и закрытие
- [x] 9. `docs/decisions/decision-log.md`
- [x] 10. `docs/reports/s001-closure.md`

### Фаза P0: Приоритетная задача Мастера
- [x] 11. Система логирования с chat-UI

## Критерии приёмки
- Все файлы созданы и содержат осмысленный контент
- Структура папок соответствует ADR-001
- Directory.Build.props запрещает обратные зависимости
- Logging system функционирует как chat-окно с вводом заметок
