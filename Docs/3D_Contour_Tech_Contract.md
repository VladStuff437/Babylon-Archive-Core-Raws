# 3D Contour Technical Contract (Session 10)

Status: active.
Scope: Start scene A-0, prologue, first three mission slices, terminal zones, hard archive entry chain.

## Goals

- Bind already-tested logic entities directly to contour-stage 3D model ids.
- Keep rendering fallback-safe: if model is unavailable, wireframe/marker rendering remains active.
- Preserve gameplay determinism while extending visual representation.

## Core Data Contract

Every logical scene object can optionally provide a model id.

- Domain: `InteractableObject.ModelId` (nullable string)
- Content: `InteractableObjectData.ModelId` (nullable string)
- Mapper: `A0ContentMapper.MapObject` forwards `ModelId`

Rules:

1. `ModelId == null` means fallback rendering is expected and valid.
2. `ModelId != null` means renderer should attempt contour model lookup first.
3. Missing model assets must never block interaction logic.
4. Interaction, phase gating, objectives, and triggers are authoritative in runtime logic, not visuals.

## Naming Convention

- Character: `CHR_*`
- Prop/interactable: `PRP_*`
- Environment/module: `ENV_*`
- Stage suffix for contour pass: `_CONTOUR`

Examples:

- `CHR_ALAN_ARCWAIN_CONTOUR`
- `PRP_CORE_CONSOLE_CONTOUR`
- `ENV_ENTRY_OCTAGON_CONTOUR`

## Axis and Scale

- World coordinates use existing Vec3 contract: X (left-right), Y (height), Z (depth).
- Unit scale: 1.0 = 1 meter.
- Contour mesh bounds should match current interaction footprint within +/- 15% tolerance.

## Visual State Mapping

Contour pass must support these states:

- `active`: default readable contour
- `focused`: highlighted contour edge
- `locked`: desaturated/low-emissive contour
- `used`: low-visibility contour (optional)

State source remains runtime (`ContextState`, phase gates, focus tracking).

## Acceptance (Block A/B Entry)

- Data model supports model id mapping end-to-end.
- Start scene object catalog has assigned contour model ids.
- Build/test remain green after model-id integration.
- No regression in input/control systems.
