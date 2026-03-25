---
name: Session orchestration task
about: Единый шаблон Issue для сессии агента в Act-1
title: "SXX | Краткое название сессии"
labels: ["type:feature", "priority:p1", "area:runtime", "stage:wave-1"]
---

Session ID: SXX  
Owner Agent: Имя агента  
Branch: feature/имя-ветки  
PR Target: integration

## Objective
Коротко, что нужно получить в конце сессии.

## Scope
Перечисли только разрешенные папки и тип работ.

## Out of scope
Что агенту запрещено менять.

## Acceptance criteria
- Что должно работать.
- Какие проверки должны быть зеленые.
- Какой результат считается готовым.

## Validation
Как проверялось: сборка, тесты, ручной сценарий.

## Risk
Кратко, где возможен побочный эффект.

## PR policy
Один Issue -> одна ветка -> один PR в `integration`.  
Без прямых коммитов в `main`.
