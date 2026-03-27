# Стандарты кода — Babylon Archive Core

## Общие принципы
- Код пишется для людей, не для компилятора
- Явное лучше неявного (кроме `var` для очевидных типов)
- Один тип — один файл (исключение: вложенные типы)
- File-scoped namespaces обязательны

## Naming

| Элемент | Стиль | Пример |
|---------|-------|--------|
| Класс, структура | PascalCase | `WorldState`, `LogEntry` |
| Интерфейс | `I` + PascalCase | `IWorldStateReader` |
| Метод | PascalCase | `GetCurrentState()` |
| Property | PascalCase | `IsActive` |
| Параметр | camelCase | `archiveAddress` |
| Локальная переменная | camelCase | `currentLevel` |
| Приватное поле | `_camelCase` | `_logService` |
| Константа | PascalCase | `MaxDepth` |
| Enum | PascalCase (тип + значения) | `LogLevel.Warning` |

## Formatting
- **Indentation**: 4 spaces (no tabs)
- **Braces**: Allman style (`{` на новой строке)
- **Line width**: 120 символов (soft limit)
- **Encoding**: UTF-8
- **Line endings**: LF
- **Trailing whitespace**: запрещён
- **Final newline**: обязателен

## C# Idioms
- `var` — когда тип очевиден из правой части
- `Nullable=enable` — всегда (Directory.Build.props)
- Pattern matching — `is Type t` вместо `as` + null check
- Records — для immutable value objects
- Expression-bodied members — для однострочников
- `using` declarations — без блока, когда одного scope достаточно
- String interpolation — вместо `String.Format`

## Документация
- XML-docs обязательны для public API (`///`)
- Комментарии — только для **почему**, не для **что**
- TODO допустимы с трекинговым номером: `// TODO(S015): optimize`

## Запреты
- `#region` запрещён (организуйте через типы/файлы)
- `dynamic` запрещён (кроме interop)
- `goto` запрещён
- Пустые catch blocks запрещены
- `Thread.Sleep` запрещён в production code
