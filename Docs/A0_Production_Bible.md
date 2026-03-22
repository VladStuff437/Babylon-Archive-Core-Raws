# A0 PRODUCTION BIBLE — Палата Инициализации A-0

> Frozen: Session 1 | 2026-03-21
> Этот документ — единственный источник истины для сцены пролога.

---

## 1. Сцена

| Поле | Значение |
|------|---------|
| SceneId | `SCN_A0_INITIATION` |
| Название (RU) | Палата Инициализации A-0 |
| Форма | Октагональный зал, вписанный в 22×22 м |
| Высота потолка | 11 м |
| Центр | Платформа C.O.R.E. |
| Ориентация | Север = +Z, Восток = +X, Верх = +Y |
| Origin | Центр платформы C.O.R.E. = (0, 0, 0) |

---

## 2. Координатная система

```
      +Z (Север)
        |
        |
-X -----+------ +X (Запад → Восток)
(Запад) |  (Восток)
        |
      -Z (Юг)

Y = высота (0 = пол, +11 = потолок)
```

Единица = 1 метр. Все координаты в мировом пространстве.

---

## 3. Зоны

| ID | Имя | Позиция (X, Y, Z) | Размер (W, H, D) | Ритм-фаза |
|----|-----|-------------------|------------------|-----------|
| `ZONE_CAPSULE` | Капсула пробуждения | (-8, 0, -6) | 4×3×3 | Awakening |
| `ZONE_BIOMETRIC` | Биометрическая станция | (-8, 0, 0) | 4×3×3 | Identification |
| `ZONE_LOGISTICS` | Логистический узел | (8, 0, -6) | 4×3×3 | Provisioning |
| `ZONE_DRONE_NICHE` | Ниша дрона | (8, 0, -2) | 3×3×3 | DroneContact |
| `ZONE_CORE` | Платформа C.O.R.E. | (0, 0, 0) | 6×6×6 | Activation |
| `ZONE_MISSION_TERMINAL` | Терминал операций | (0, 0, 8) | 4×3×4 | OperationAccess |
| `ZONE_RESEARCH` | Терминал исследований | (6, 0, 6) | 4×3×4 | OperationAccess |
| `ZONE_OBSERVATION_GALLERY` | Обзорная галерея | (0, 0, 10) | 8×3×2 | OperationAccess |
| `ZONE_HARD_ARCHIVE_ENTRY_LOCKED` | Вход в Хард-Архив (закрыт) | (0, -1, 11) | 4×5×2 | OperationAccess |
| `ZONE_COMMERCE_GATE_LOCKED` | Коммерческий шлюз (закрыт) | (-6, 0, 6) | 3×3×3 | — (v2) |
| `ZONE_TECH_GATE_LOCKED` | Технический шлюз (закрыт) | (0, 0, -8) | 3×3×3 | — (v2) |

---

## 4. Интерактивные объекты

| ObjectId | Зона | Тип | Hint | Фаза |
|----------|------|-----|------|------|
| `OBJ_CAPSULE_EXIT` | ZONE_CAPSULE | trigger | «Нажми [E] чтобы выйти» | Awakening |
| `OBJ_BIO_SCANNER` | ZONE_BIOMETRIC | terminal | «Приложи руку для идентификации» | Identification |
| `OBJ_SUPPLY_TERMINAL` | ZONE_LOGISTICS | terminal | «Получи снаряжение» | Provisioning |
| `OBJ_DRONE_DOCK` | ZONE_DRONE_NICHE | npc | «Дрон ожидает инициализации» | DroneContact |
| `OBJ_CORE_CONSOLE` | ZONE_CORE | terminal | «Доступ к системам Архива» | Activation |
| `OBJ_OP_TERMINAL` | ZONE_MISSION_TERMINAL | terminal | «Журнал операций» | OperationAccess |
| `OBJ_RESEARCH_TERMINAL` | ZONE_RESEARCH | terminal | «Исследования (ограничено)» | OperationAccess |
| `OBJ_GALLERY_OVERLOOK` | ZONE_OBSERVATION_GALLERY | trigger | «Посмотри вниз» | OperationAccess |
| `OBJ_ARCHIVE_GATE` | ZONE_HARD_ARCHIVE_ENTRY_LOCKED | gate | «Доступ заблокирован» | OperationAccess |

---

## 5. Обязательный маршрут пролога

```
1. [Awakening]      → OBJ_CAPSULE_EXIT       → Алан выходит из капсулы
2. [Identification]  → OBJ_BIO_SCANNER        → Идентификация: Алан Арквейн
3. [Provisioning]    → OBJ_SUPPLY_TERMINAL    → Стартовое снаряжение получено
4. [DroneContact]    → OBJ_DRONE_DOCK         → Дрон активирован, первый диалог
5. [Activation]      → OBJ_CORE_CONSOLE       → C.O.R.E. активирован
6. [OperationAccess] → OBJ_OP_TERMINAL        → Древо операций показано
7. [OperationAccess] → OBJ_GALLERY_OVERLOOK   → Взгляд вниз на Хард-Архив
8. [OperationAccess] → OBJ_ARCHIVE_GATE       → «Доступ будет открыт после первой операции»
9. ФИНАЛ             → Протокол Ноль разблокирован
```

Порядок шагов 1–5 строгий. Шаги 6–8 доступны одновременно после Activation.

---

## 6. Ритм-фазы (обновлённые)

| Фаза | Enum | Триггер перехода |
|------|------|-----------------|
| Пробуждение | `Awakening` | Старт сцены |
| Идентификация | `Identification` | OBJ_CAPSULE_EXIT |
| Снабжение | `Provisioning` | OBJ_BIO_SCANNER |
| Контакт с дроном | `DroneContact` | OBJ_SUPPLY_TERMINAL |
| Активация | `Activation` | OBJ_DRONE_DOCK |
| Доступ к операциям | `OperationAccess` | OBJ_CORE_CONSOLE |

---

## 7. Стартовый профиль оператора

| Поле | Значение |
|------|---------|
| Имя | Алан Арквейн (Alan Arcwain) |
| Уровень | 1 |
| XP | 0 |
| Analysis | 10 |
| Influence | 10 |
| Tech | 10 |
| Arcane | 10 |
| Endurance | 10 |

### Стартовый инвентарь

| ItemId | Имя | Тип |
|--------|-----|-----|
| `ITEM_ARCHIVE_BADGE` | Архивный жетон | key_item |
| `ITEM_BASIC_SCANNER` | Базовый сканер | tool |
| `ITEM_FIELD_RATION` | Полевой рацион | consumable |

---

## 8. Приоритеты ассетов

| Уровень | Тип | Статус в v1 |
|---------|-----|-------------|
| P0 | Геометрия зала (white-box) | обязательно |
| P0 | Коллизии | обязательно |
| P0 | Интерактивные точки (маркеры) | обязательно |
| P1 | Базовое освещение | нужно |
| P1 | Простые модели терминалов | нужно |
| P2 | Пар капсулы, голограммы CORE | желательно |
| P2 | Эмбиент-звуки | желательно |
| P3 | Финальный арт, детализация | v2+ |

---

## 9. Визуальная шахта Хард-Архива

Из обзорной галереи видна вертикальная шахта вниз:
- Глубина видимости: 3 фальш-этажа
- Центральный Void: ø8 м
- VoidScale = 0.72
- Стены шахты: полки, коридоры, лестницы (всё — декорация в v1)
- Свет: затухающий с глубиной, красноватые отблески

---

## 10. Канон, применимый к v1

- Движок: MonoGame + Babylon Runtime
- 3D первичный, 2.5D — camera profile
- Контент в JSON, не в коде
- Провал ≠ Game Over (но в прологе провала нет)
- Тексты: короткие и сильные
- Дрон — обязательный атмосферный NPC
- C.O.R.E. — главный сюжетный узел сцены
