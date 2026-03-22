# NAMING CONVENTION — Babylon Archive Core

> Frozen: Session 1 | 2026-03-21

---

## 1. Префиксы идентификаторов

| Префикс | Область | Пример |
|---------|---------|--------|
| `SCN_` | Сцена (Scene) | `SCN_A0_INITIATION` |
| `ROOM_` | Комната / помещение | `ROOM_CAPSULE_CHAMBER` |
| `ZONE_` | Зона внутри сцены | `ZONE_CORE` |
| `OBJ_` | Интерактивный объект | `OBJ_BIO_SCANNER` |
| `TRG_` | Триггер (событийный) | `TRG_PHASE_ADVANCE` |
| `DIA_` | Диалог / реплика | `DIA_DRONE_GREETING` |
| `UI_` | Экранный элемент | `UI_OBJECTIVE_PANEL` |
| `ITEM_` | Предмет инвентаря | `ITEM_BASIC_SCANNER` |
| `NPC_` | Персонаж / сущность | `NPC_ARCHIVE_DRONE` |
| `SFX_` | Звуковой эффект | `SFX_CAPSULE_OPEN` |
| `BGM_` | Фоновая музыка | `BGM_HUB_AMBIENT` |
| `FX_` | Визуальный эффект | `FX_CORE_HOLOGRAM` |

---

## 2. Правила

1. **SCREAMING_SNAKE_CASE** для всех ID в JSON-контенте и константах.
2. **PascalCase** для C# типов, свойств, методов.
3. **camelCase** для C# локальных переменных и параметров.
4. Префикс всегда отделяется от имени символом `_`.
5. Имена — на **английском**. Тексты — на **русском** (v1).
6. Не использовать пробелы, дефисы, точки в ID.
7. Длина ID: макс. 40 символов.

---

## 3. Архивная адресация

Формат:
```
Sector-Hall-Module-Shelf-Cell-Tome-Page-Branch
```

Пример:
```
A0-H1-M03-S2-C5-T01-P07-B2
```

| Поле | Формат | Описание |
|------|--------|----------|
| Sector | `A0` | Сектор архива (буква + номер) |
| Hall | `H1` | Зал в секторе |
| Module | `M03` | Модуль (шкаф / блок) |
| Shelf | `S2` | Полка в модуле |
| Cell | `C5` | Ячейка на полке |
| Tome | `T01` | Том (книга) |
| Page | `P07` | Страница (= миссия) |
| Branch | `B2` | Ветвь решения |

### Маппинг на существующий ArchiveAddress

Текущий `ArchiveAddress` в коде использует числовой формат:
`S00.H00.M00.SH00.C00.T000.P000`

Новый формат — для дизайн-документов и JSON. Конвертация:
- `A0-H1-M03-S2-C5-T01-P07` → `ArchiveAddress(Sector:0, Hall:1, Module:3, Shelf:2, Cell:5, Tome:1, Page:7)`

Branch не хранится в ArchiveAddress — он часть MissionNode.

---

## 4. Файловая структура контента

```
Content/
├── Scenes/
│   └── SCN_A0_INITIATION.json
├── Zones/
│   └── A0_Zones.json
├── Objects/
│   └── A0_Objects.json
├── Dialogue/
│   └── A0_Dialogue.json
├── Triggers/
│   └── A0_Triggers.json
└── UI/
    └── A0_Objectives.json
```

Все JSON-файлы — UTF-8 без BOM, отступ: 2 пробела.

---

## 5. C# Namespace Convention

| Проект | Корневой namespace |
|--------|-------------------|
| Domain | `BabylonArchiveCore.Domain` |
| Core | `BabylonArchiveCore.Core` |
| Runtime | `BabylonArchiveCore.Runtime` |
| Content | `BabylonArchiveCore.Content` |
| Infrastructure | `BabylonArchiveCore.Infrastructure` |
| Presentation | `BabylonArchiveCore.Presentation` |
| Tests | `BabylonArchiveCore.Tests` |

Вложенные папки → вложенные namespace.
Пример: `BabylonArchiveCore.Domain.Scene`
