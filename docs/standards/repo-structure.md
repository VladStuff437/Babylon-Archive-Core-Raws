# Стандарт структуры репозитория — Babylon Archive Core

## Корневые директории

| Директория | Назначение | Правила |
|-----------|-----------|---------|
| `src/` | Исходный код (5 проектов) | Только .csproj проекты слоёв |
| `tests/` | Тестовые проекты | xUnit, сессионные папки |
| `docs/` | Документация | ADR, architecture, checklists, reports, standards |
| `tools/` | Утилиты, скрипты | Помощники разработки |
| `MasterPlan/` | MasterPlan.md | Только source of truth |

## Конвенции именования

### Код
- **Namespace**: `BabylonArchiveCore.{Layer}.{Subsystem}`
- **Класс/Интерфейс**: PascalCase, один тип — один файл
- **Интерфейсы**: `I` + PascalCase (`IWorldStateReader`)
- **Папки в проекте**: PascalCase по подсистеме

### Документация
- **ADR**: `ADR-NNN.md` (NNN = 3 цифры)
- **Сессии**: `s{NNN}.md` (строчная s)
- **Прочие docs**: kebab-case (`layer-responsibilities.md`)
- **Чеклисты**: `s{NNN}-implementation.md`
- **Reports**: `s{NNN}-closure.md`

### Тесты
- **Папки**: `Session{NNN}/`
- **Файлы**: `Session{NNN}Tests.cs`
- **Классы**: `Session{NNN}Tests`

## Запрещено
- `.cs` файлы в корне репозитория
- `.csproj` вне `src/` и `tests/`
- Binary/build артефакты (bin/, obj/, *.dll)
- IDE-специфичные файлы (кроме .editorconfig)

## Обязательные корневые файлы
- `BabylonArchiveCore.sln`
- `Directory.Build.props`
- `.gitignore`
- `.editorconfig` (с S008)
- `README.md`
