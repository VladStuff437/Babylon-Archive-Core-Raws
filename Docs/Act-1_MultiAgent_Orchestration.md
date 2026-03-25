# Act-1 Multi-Agent Orchestration

Ниже готовые блоки для копирования и единая карта оркестрации по сессиям.

## Блок 1. Шаблон Issue (GitHub Issues -> New issue)

**TITLE:**  
`SXX | Краткое название сессии`

**LABELS:**  
`type:feature, priority:p1, area:runtime, stage:wave-1`

**MILESTONE:**  
`Act-1-Orchestration`

**BODY:**

```text
Session ID: SXX
Owner Agent: Имя агента
Branch: feature/имя-ветки
PR Target: integration

Objective:
Коротко, что нужно получить в конце сессии.

Scope:
Перечисли только разрешенные папки и тип работ.

Out of scope:
Что агенту запрещено менять.

Acceptance criteria:
- Что должно работать.
- Какие проверки должны быть зеленые.
- Какой результат считается готовым.

Validation:
Как проверялось: сборка, тесты, ручной сценарий.

Risk:
Кратко, где возможен побочный эффект.

PR policy:
Один Issue -> одна ветка -> один PR в integration.
Без прямых коммитов в main.
```

## Блок 2. Шаблон первой команды в сессию агента

```text
Ты работаешь только в рамках одной сессии.

Session ID: SXX
Issue: #НОМЕР_ISSUE
Branch: feature/имя-ветки
PR Target: integration

Разрешенные зоны:
- Папка 1
- Папка 2
- Папка 3

Запрещено:
- Менять любые файлы вне разрешенных зон
- Коммитить артефакты сборки и логи
- Делать PR в main

Что сделать:
- Выполнить только задачу из Issue.
- Добавить или обновить тесты по измененному поведению.
- Открыть один Draft PR в integration.
- В описании PR указать:
  - что сделано
  - как проверено
  - риски
  - что осталось

Критерий завершения:
Green checks и готовый PR по этому Issue.
```

## Блок 3. Быстрый выбор пары Session + Branch + цель

- `S00` -> `chore/ci-actions-bootstrap` -> CI bootstrap and branch safety
- `S01` -> `chore/repo-hygiene` -> Repository hygiene and ignore hardening
- `S02` -> `chore/docs-process` -> Process docs for multi-agent orchestration
- `S10` -> `feature/control-v2` -> Control V2 stabilization
- `S11` -> `feature/runtime-world-a0` -> Runtime world flow and scene interactions
- `S12` -> `feature/content-pipeline-a0` -> Content pipeline and registry hardening
- `S13` -> `feature/desktop-client-a0` -> Desktop client UX and input-focus integration
- `S14` -> `feature/test-regression-a0` -> Regression matrix and test hardening
- `S20` -> `release/act-1-a0` -> Release stabilization and promotion to main

## Роли агентов (рекомендация по оркестрации)

- **Agent-CI (S00):** workflows, protections, checks.
- **Agent-Hygiene (S01):** `.gitignore`, cleanup policy, repo hygiene.
- **Agent-Process (S02):** issue/pr templates, orchestration docs.
- **Agent-Runtime (S10/S11):** runtime/control/gameplay flow.
- **Agent-Content (S12):** content schemas, registries, pipeline safety.
- **Agent-Desktop (S13):** desktop UX/input focus integration.
- **Agent-QA (S14):** regression matrix and hardening tests.
- **Release-Agent (S20):** stabilization and promotion flow.

## Готовая раскладка Issue + Branch + Session + PR

> Все PR: **Draft PR -> target `integration`**.

### S00 — CI bootstrap and branch safety
- **Issue title:** `S00 | CI bootstrap and branch safety`
- **Branch:** `chore/ci-actions-bootstrap`
- **Owner Agent:** Agent-CI
- **PR Target:** `integration`
- **Allowed zones:** `.github/workflows`, `.github` policy files
- **Out of scope:** runtime/content/desktop код

### S01 — Repository hygiene and ignore hardening
- **Issue title:** `S01 | Repository hygiene and ignore hardening`
- **Branch:** `chore/repo-hygiene`
- **Owner Agent:** Agent-Hygiene
- **PR Target:** `integration`
- **Allowed zones:** `.gitignore`, repo hygiene docs/scripts
- **Out of scope:** gameplay/runtime logic

### S10 — Control V2 stabilization
- **Issue title:** `S10 | Control V2 stabilization`
- **Branch:** `feature/control-v2`
- **Owner Agent:** Agent-Runtime
- **PR Target:** `integration`
- **Allowed zones:** `BabylonArchiveCore.Runtime`, `BabylonArchiveCore.Core`, соответствующие тесты
- **Out of scope:** CI и release-пайплайны

### S11 — Runtime world flow and scene interactions
- **Issue title:** `S11 | Runtime world flow and scene interactions`
- **Branch:** `feature/runtime-world-a0`
- **Owner Agent:** Agent-Runtime
- **PR Target:** `integration`
- **Allowed zones:** runtime flow/scene interaction модули + тесты
- **Out of scope:** desktop UX и release

### S12 — Content pipeline and registry hardening
- **Issue title:** `S12 | Content pipeline and registry hardening`
- **Branch:** `feature/content-pipeline-a0`
- **Owner Agent:** Agent-Content
- **PR Target:** `integration`
- **Allowed zones:** `Content/**`, `BabylonArchiveCore.Content`, `BabylonArchiveCore.Infrastructure`, тесты pipeline
- **Out of scope:** desktop UI и CI orchestration

### S13 — Desktop client UX and input-focus integration
- **Issue title:** `S13 | Desktop client UX and input-focus integration`
- **Branch:** `feature/desktop-client-a0`
- **Owner Agent:** Agent-Desktop
- **PR Target:** `integration`
- **Allowed zones:** `BabylonArchiveCore.Desktop`, presentation-слой, desktop тесты
- **Out of scope:** content pipeline и release promotion

### S14 — Regression matrix and test hardening
- **Issue title:** `S14 | Regression matrix and test hardening`
- **Branch:** `feature/test-regression-a0`
- **Owner Agent:** Agent-QA
- **PR Target:** `integration`
- **Allowed zones:** `BabylonArchiveCore.Tests`, QA docs
- **Out of scope:** прод-код без явной причины

### S20 — Release stabilization and promotion to main
- **Issue title:** `S20 | Release stabilization and promotion to main`
- **Branch:** `release/act-1-a0`
- **Owner Agent:** Release-Agent
- **PR Target:** `integration` (далее promotion-процедура в `main`)
- **Allowed zones:** release notes, final checks, stabilization fixes
- **Out of scope:** новые фичи

## Правила веток, Issue и PR

1. Один Issue = одна ветка = один PR.
2. Все PR только в `integration`.
3. В `main` прямые коммиты запрещены.
4. Каждый PR должен содержать:
   - что сделано;
   - как проверено;
   - риски;
   - что осталось.
5. Merge только при green checks.

## Готовые Issue-тексты (S00, S01, S10, S11, S12, S13)

> Ниже можно копировать в GitHub Issues без редактирования (кроме номера Issue и имени агента).

### S00

**TITLE:** `S00 | CI bootstrap and branch safety`  
**LABELS:** `type:feature, priority:p1, area:runtime, stage:wave-1`  
**MILESTONE:** `Act-1-Orchestration`

```text
Session ID: S00
Owner Agent: Agent-CI
Branch: chore/ci-actions-bootstrap
PR Target: integration

Objective:
Стабилизировать CI и зафиксировать безопасный branch-flow для integration/main.

Scope:
- .github/workflows
- .github policy/config файлы
- минимально необходимые process docs

Out of scope:
- runtime/content/desktop код

Acceptance criteria:
- CI запускается на integration/main и не ломает текущий pipeline.
- Проверки branch safety применяются для target integration.
- Draft PR в integration содержит прозрачную валидацию.

Validation:
Проверка workflow-конфигурации, локальная валидация YAML/триггеров, прогон имеющихся тестов.

Risk:
Неверная настройка триггеров может заблокировать merge в integration.

PR policy:
Один Issue -> одна ветка -> один PR в integration.
Без прямых коммитов в main.
```

### S01

**TITLE:** `S01 | Repository hygiene and ignore hardening`  
**LABELS:** `type:feature, priority:p1, area:runtime, stage:wave-1`  
**MILESTONE:** `Act-1-Orchestration`

```text
Session ID: S01
Owner Agent: Agent-Hygiene
Branch: chore/repo-hygiene
PR Target: integration

Objective:
Усилить гигиену репозитория: исключить артефакты, логи и временные файлы из PR.

Scope:
- .gitignore
- process/docs по чистоте репозитория
- минимальные служебные правила для clean PR

Out of scope:
- изменение бизнес-логики runtime/content/desktop

Acceptance criteria:
- Build/test артефакты не попадают в git status после типового прогона.
- Политика hygiene описана и понятна для агентных сессий.
- Все проверки остаются зелеными.

Validation:
Локальный прогон build/test и проверка git status, ручная проверка .gitignore.

Risk:
Слишком агрессивные ignore-правила могут скрыть нужные исходники.

PR policy:
Один Issue -> одна ветка -> один PR в integration.
Без прямых коммитов в main.
```

### S10

**TITLE:** `S10 | Control V2 stabilization`  
**LABELS:** `type:feature, priority:p1, area:runtime, stage:wave-1`  
**MILESTONE:** `Act-1-Orchestration`

```text
Session ID: S10
Owner Agent: Agent-Runtime
Branch: feature/control-v2
PR Target: integration

Objective:
Стабилизировать Control V2 и убрать критичные регрессии в управлении.

Scope:
- BabylonArchiveCore.Runtime
- BabylonArchiveCore.Core (только связанный control-код)
- BabylonArchiveCore.Tests (целевые тесты control)

Out of scope:
- CI/infra и desktop UX изменения, не связанные с control.

Acceptance criteria:
- Control V2 работает предсказуемо в основных сценариях.
- Добавлены/обновлены тесты на измененное поведение.
- Целевые проверки и тесты зеленые.

Validation:
Таргетный прогон тестов control/runtime + ручной сценарий поведения.

Risk:
Изменения в control-пайплайне могут затронуть существующие input-сценарии.

PR policy:
Один Issue -> одна ветка -> один PR в integration.
Без прямых коммитов в main.
```

### S11

**TITLE:** `S11 | Runtime world flow and scene interactions`  
**LABELS:** `type:feature, priority:p1, area:runtime, stage:wave-1`  
**MILESTONE:** `Act-1-Orchestration`

```text
Session ID: S11
Owner Agent: Agent-Runtime
Branch: feature/runtime-world-a0
PR Target: integration

Objective:
Довести world flow сцены A0 и ключевые scene-interactions до стабильного состояния.

Scope:
- BabylonArchiveCore.Runtime (world flow / interactions)
- BabylonArchiveCore.Core (только необходимые зависимости)
- BabylonArchiveCore.Tests (runtime-flow тесты)

Out of scope:
- content registry hardening, desktop UX и release-задачи.

Acceptance criteria:
- Критические переходы world flow отрабатывают корректно.
- Взаимодействия со сценой не ломают существующие сценарии.
- Green checks по релевантным тестам.

Validation:
Таргетные тесты runtime world flow + ручной прогон сценового сценария.

Risk:
Побочные эффекты на последовательности триггеров и переходы фаз.

PR policy:
Один Issue -> одна ветка -> один PR в integration.
Без прямых коммитов в main.
```

### S12

**TITLE:** `S12 | Content pipeline and registry hardening`  
**LABELS:** `type:feature, priority:p1, area:runtime, stage:wave-1`  
**MILESTONE:** `Act-1-Orchestration`

```text
Session ID: S12
Owner Agent: Agent-Content
Branch: feature/content-pipeline-a0
PR Target: integration

Objective:
Укрепить content pipeline и регистры, чтобы исключить хрупкие кейсы загрузки/валидации.

Scope:
- Content/**
- BabylonArchiveCore.Content
- BabylonArchiveCore.Infrastructure (только pipeline/registry слой)
- BabylonArchiveCore.Tests (pipeline тесты)

Out of scope:
- desktop UX, CI orchestration и release promotion.

Acceptance criteria:
- Pipeline устойчив к базовым ошибкам контента.
- Registry корректно валидирует/резолвит контент.
- Тесты pipeline и регрессии зеленые.

Validation:
Таргетные unit/integration тесты контент-пайплайна + ручная проверка загрузки.

Risk:
Ужесточение валидации может отфильтровать ранее допустимые данные.

PR policy:
Один Issue -> одна ветка -> один PR в integration.
Без прямых коммитов в main.
```

### S13

**TITLE:** `S13 | Desktop client UX and input-focus integration`  
**LABELS:** `type:feature, priority:p1, area:runtime, stage:wave-1`  
**MILESTONE:** `Act-1-Orchestration`

```text
Session ID: S13
Owner Agent: Agent-Desktop
Branch: feature/desktop-client-a0
PR Target: integration

Objective:
Улучшить UX desktop-клиента и стабилизировать input-focus интеграцию.

Scope:
- BabylonArchiveCore.Desktop
- BabylonArchiveCore.Presentation (только desktop-связанный код)
- BabylonArchiveCore.Tests (desktop/input-focus тесты)

Out of scope:
- content pipeline hardening и release-процедуры.

Acceptance criteria:
- Input focus корректен в ключевых desktop-сценариях.
- UX улучшения не ломают текущие потоки.
- Green checks по desktop- и смежным тестам.

Validation:
Таргетный тестовый прогон + ручной desktop-сценарий с проверкой focus/input.

Risk:
Регрессии в обработке ввода при переключении фокуса окна.

PR policy:
Один Issue -> одна ветка -> один PR в integration.
Без прямых коммитов в main.
```
