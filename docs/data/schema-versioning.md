# Schema Versioning Policy

## Актуальные версии
- Session011: schema version 11
- Session012: schema version 12
- Session013: schema version 13
- Session014: schema version 14
- Session015: schema version 15
- Session016: schema version 16
- Session017: schema version 17
- Session018: schema version 18
- Session019: schema version 19
- Session020: schema version 20

## Правила
- Каждое изменение структуры данных повышает schemaVersion.
- Для каждой новой версии добавляется migration шаг в runtime.
- Минимально совместимая версия фиксируется отдельно от текущей.

## Migration flow
1. Detect legacy schema version.
2. Sequentially apply migration steps.
3. Validate against current JSON schema.
4. Persist with latest schemaVersion.

## Реализованные migration steps
- Migration_011
- Migration_012
- Migration_013
- Migration_014
- Migration_015
- Migration_016
- Migration_017
- Migration_018
- Migration_019
- Migration_020
