# 3D Model Registry: A-0 Start Scene

This registry binds tested logical object ids to contour-stage model ids.

## Interactable Objects

| ObjectId | Zone | Phase | ModelId | Description |
| --- | --- | --- | --- | --- |
| `capsule_exit` | Capsule | Awakening | `PRP_CAPSULE_EXIT_CONTOUR` | Capsule release chamber exit frame |
| `bio_scanner` | Biometrics | Identification | `PRP_BIO_SCANNER_CONTOUR` | Vertical scanner stand with status head |
| `supply_terminal` | Logistics | Provisioning | `PRP_SUPPLY_TERMINAL_CONTOUR` | Equipment issue kiosk |
| `drone_dock` | DroneNiche | DroneContact | `PRP_DRONE_DOCK_CONTOUR` | Docking pad and niche anchor |
| `core_console` | Core | Activation | `PRP_CORE_CONSOLE_CONTOUR` | Central CORE operations console |
| `op_terminal` | MissionTerminal | OperationAccess | `PRP_MISSION_TERMINAL_CONTOUR` | Mission board terminal station |
| `research_terminal` | Research | OperationAccess | `PRP_RESEARCH_TERMINAL_CONTOUR` | Research access terminal |
| `gallery_overlook` | ObservationGallery | OperationAccess | `PRP_GALLERY_OVERLOOK_CONTOUR` | Observation point contour marker |
| `archive_gate` | HardArchiveEntrance | OperationAccess | `PRP_ARCHIVE_GATE_CONTOUR` | Hard archive gate and lock frame |

## Hero and Key Scene Nodes

| Entity | ModelId | Description |
| --- | --- | --- |
| Alan Arcwain | `CHR_ALAN_ARCWAIN_CONTOUR` | Readable silhouette for movement and facing |
| Mission Board | `PRP_MISSION_BOARD_CONTOUR` | Three-slot mission panel |
| Research Lab | `PRP_RESEARCH_LAB_CONTOUR` | Analyst workstation cluster |
| Tool Bench | `PRP_TOOL_BENCH_CONTOUR` | Workshop contour bench |
| Archive Control | `PRP_ARCHIVE_CONTROL_CONTOUR` | Control module before archive chain |
| Commerce Desk | `PRP_COMMERCE_DESK_CONTOUR` | Service module for commerce simulation |

## Extra Interaction Nodes

| Node | ModelId | Description |
| --- | --- | --- |
| commerce_hall | `ENV_COMMERCE_HALL_CONTOUR` | Commerce access hall marker |
| tech_hall | `ENV_TECH_HALL_CONTOUR` | Technical access hall marker |
| archive_preview | `ENV_ARCHIVE_PREVIEW_CONTOUR` | Hard archive preview contour gateway |
| research_lab | `PRP_RESEARCH_LAB_CONTOUR` | Analyst workstation cluster |
| tool_bench | `PRP_TOOL_BENCH_CONTOUR` | Workshop contour bench |
| commerce_desk | `PRP_COMMERCE_DESK_CONTOUR` | Service module terminal cluster |

## Hard Archive Entry Chain

| Node | ModelId | Description |
| --- | --- | --- |
| archive_corridor | `ENV_ARCHIVE_CORRIDOR_CONTOUR` | Transition corridor module |
| entry_octagon | `ENV_ENTRY_OCTAGON_CONTOUR` | Eight-sided entry chamber |
| index_vestibule | `ENV_INDEX_VESTIBULE_CONTOUR` | Index vestibule transition node |
| research_room_01 | `ENV_RESEARCH_ROOM_01_CONTOUR` | Research room shell with console |
| stack_ring_preview | `ENV_STACK_RING_PREVIEW_CONTOUR` | Stack ring preview with central void marker |

## Notes

- Registry is contour-stage only and may evolve in detailed art pass.
- Model ids are stable identifiers for telemetry and automated render smoke snapshots.
