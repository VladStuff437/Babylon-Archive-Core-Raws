# Control V2 Specification

Status: approved baseline for Session 10 desktop harness rewrite.

## Goals

- Deterministic control semantics with no hidden reverse flags.
- Stable camera behavior: behind-player follow with optional RMB orbit modifier.
- Strict separation between UI input and gameplay input.
- Settings in menu reflect active runtime behavior only.

## Input Behavior Table

| Input | Condition | Expected Behavior |
| --- | --- | --- |
| `W` | Gameplay input enabled | Move player forward relative to camera yaw |
| `S` | Gameplay input enabled | Move player backward relative to camera yaw |
| `A` | Gameplay input enabled | Strafe left relative to camera yaw |
| `D` | Gameplay input enabled | Strafe right relative to camera yaw |
| `RMB drag X` | RMB held and gameplay input enabled | Modify camera orbit offset around player |
| `RMB release` | RMB released | Keep deterministic camera yaw, no jump |
| `E` | Gameplay input enabled | Interact with focused target |
| `Q` | Gameplay input enabled | Toggle camera mode as defined by gameplay session |
| `Space` | Gameplay input enabled | Advance dialogue |
| `Esc` | Any | Close overlay/menu if open, else open settings |
| `R` | Gameplay input enabled | Reset runtime session |
| Text input keys | Note input focused | Never trigger movement/turn/interact |

## Architecture Layers

1. Input Capture Layer
- Responsible for raw keys/mouse capture and UI focus gate.
- Produces signals for movement, turn, and camera orbit modifier.

2. Movement Model Layer
- Responsible for deterministic camera-relative movement.
- `W/S` map to camera-relative forward/back.
- `A/D` map to camera-relative strafe.
- Player facing is updated from non-zero movement direction.

3. Camera Model Layer
- Responsible for follow yaw and orbit offset composition.
- Formula: `targetYaw = playerYaw + orbitOffsetYaw`.
- Smoothing occurs in one place only.

## ControllerState Contract

ControllerState is the single source of truth for runtime control state:

- `PlayerYaw`
- `OrbitOffsetYaw`
- `CameraYawSmoothed`
- `IsOrbitModifierActive`
- `IsUiInputCaptured`

All camera yaw calculations must flow through ControllerState.

## Settings Surface (Control V2)

Allowed Control V2 settings:

- `OrbitSensitivity`
- `CameraSmoothing`
- `MaxOrbitOffsetYaw`
- `InvertOrbitHorizontal`
- `InvertOrbitVertical` (reserved for pitch pipeline)
- `Deadzone`

Deprecated / removed from control settings:

- Hidden reverse toggles
- Legacy control mode switches that do not map to active code path

## Telemetry Requirements

Log control telemetry on:

- state changes (orbit/camera settings updates, UI capture on/off)
- movement/camera heartbeat interval

Each heartbeat should include at minimum:

- current phase
- player position
- `playerYaw`
- `cameraYaw`
- `orbitOffsetYaw`
- `uiCapture`
- pause menu state

## Acceptance

- `TickFrame` orchestrates only and delegates control math.
- Camera follow remains stable during movement with orbit recentering when RMB is not held.
- UI input focus never emits gameplay movement commands.
- Control settings persist between restarts.
