using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Domain.Scene.Geometry;

namespace BabylonArchiveCore.Runtime.Scene;

/// <summary>
/// Constructs the canonical Hub A-0 scene with all zones and interactable objects.
/// Session 2: real 3D coordinates from A0_Production_Bible.md.
/// Origin = C.O.R.E. platform centre. X=east, Y=up, Z=north. 1 unit = 1 metre.
/// Flow: Capsule → Biometrics → Logistics → Drone → Core → Terminals / Gallery / Archive.
/// </summary>
public static class HubA0SceneBuilder
{
    /// <summary>Octagonal hall inscribed in 22×22 m, 11 m ceiling.</summary>
    public const float HallSize = 22f;
    public const float CeilingHeight = 11f;

    /// <summary>Build the floor geometry for A-0.</summary>
    public static OctagonalFloor BuildFloor() => new(HallSize, CeilingHeight);

    public static List<HubZone> Build()
    {
        return
        [
            // --- Zone positions and sizes from A0_Production_Bible §3 ---
            MakeZone(HubZoneId.Capsule, "Капсула пробуждения",
                -8f, 0f, -6f, 4f, 3f, 3f,
                new InteractableObject
                {
                    Id = "capsule_exit",
                    DisplayName = "Выход из капсулы",
                    HintText = "Нажмите [Interact] чтобы выйти",
                    Zone = HubZoneId.Capsule,
                    Position = new Vec3(-7f, 0.5f, -6f),
                    Bounds = new Bounds3D(-7.5f, 0f, -6.5f, 1f, 2f, 1f),
                    InteractionRadius = 1.5f,
                    RequiredPhase = HubRhythmPhase.Awakening,
                    InteractiveType = InteractiveType.Trigger,
                }),

            MakeZone(HubZoneId.Biometrics, "Биометрическая станция",
                -8f, 0f, 0f, 4f, 3f, 3f,
                new InteractableObject
                {
                    Id = "bio_scanner",
                    DisplayName = "Биосканер",
                    HintText = "Приложите руку для идентификации",
                    Zone = HubZoneId.Biometrics,
                    Position = new Vec3(-8f, 1f, 0f),
                    Bounds = new Bounds3D(-8.5f, 0f, -0.5f, 1f, 2f, 1f),
                    InteractionRadius = 1.5f,
                    RequiredPhase = HubRhythmPhase.Identification,
                    InteractiveType = InteractiveType.Terminal,
                    DialogueId = "DIA_BIO_CONFIRM",
                }),

            MakeZone(HubZoneId.Logistics, "Логистический узел",
                8f, 0f, -6f, 4f, 3f, 3f,
                new InteractableObject
                {
                    Id = "supply_terminal",
                    DisplayName = "Терминал снабжения",
                    HintText = "Получите снаряжение на смену",
                    Zone = HubZoneId.Logistics,
                    Position = new Vec3(8f, 1f, -6f),
                    Bounds = new Bounds3D(7.5f, 0f, -6.5f, 1f, 2f, 1f),
                    InteractionRadius = 1.5f,
                    RequiredPhase = HubRhythmPhase.Provisioning,
                    InteractiveType = InteractiveType.Terminal,
                    DialogueId = "DIA_SUPPLY_GRANTED",
                    GrantsItems = ["ITEM_ARCHIVE_BADGE", "ITEM_BASIC_SCANNER", "ITEM_FIELD_RATION"],
                }),

            MakeZone(HubZoneId.DroneNiche, "Ниша дрона",
                8f, 0f, -2f, 3f, 3f, 3f,
                new InteractableObject
                {
                    Id = "drone_dock",
                    DisplayName = "Стыковочная ниша дрона",
                    HintText = "Дрон ожидает инициализации",
                    Zone = HubZoneId.DroneNiche,
                    Position = new Vec3(8f, 1.2f, -2f),
                    Bounds = new Bounds3D(7.5f, 0f, -2.5f, 1f, 2f, 1f),
                    InteractionRadius = 2f,
                    RequiredPhase = HubRhythmPhase.DroneContact,
                    InteractiveType = InteractiveType.Npc,
                    DialogueId = "DIA_DRONE_GREETING",
                }),

            MakeZone(HubZoneId.Core, "C.O.R.E.",
                0f, 0f, 0f, 6f, 6f, 6f,
                new InteractableObject
                {
                    Id = "core_console",
                    DisplayName = "Консоль C.O.R.E.",
                    HintText = "Доступ к системам Архива",
                    Zone = HubZoneId.Core,
                    Position = new Vec3(0f, 1.5f, 0f),
                    Bounds = new Bounds3D(-0.5f, 0f, -0.5f, 1f, 3f, 1f),
                    InteractionRadius = 2.5f,
                    RequiredPhase = HubRhythmPhase.Activation,
                    InteractiveType = InteractiveType.Terminal,
                    DialogueId = "DIA_CORE_ACTIVATION",
                }),

            MakeZone(HubZoneId.MissionTerminal, "Терминал операций",
                0f, 0f, 8f, 4f, 3f, 4f,
                new InteractableObject
                {
                    Id = "op_terminal",
                    DisplayName = "Терминал операций",
                    HintText = "Журнал операций и назначения",
                    Zone = HubZoneId.MissionTerminal,
                    Position = new Vec3(0f, 1f, 8f),
                    Bounds = new Bounds3D(-0.5f, 0f, 7.5f, 1f, 2f, 1f),
                    InteractionRadius = 1.5f,
                    RequiredPhase = HubRhythmPhase.OperationAccess,
                    InteractiveType = InteractiveType.Terminal,
                }),

            MakeZone(HubZoneId.Research, "Терминал исследований",
                6f, 0f, 6f, 4f, 3f, 4f,
                new InteractableObject
                {
                    Id = "research_terminal",
                    DisplayName = "Терминал исследований",
                    HintText = "Исследования (доступ ограничен)",
                    Zone = HubZoneId.Research,
                    Position = new Vec3(6f, 1f, 6f),
                    Bounds = new Bounds3D(5.5f, 0f, 5.5f, 1f, 2f, 1f),
                    InteractionRadius = 1.5f,
                    RequiredPhase = HubRhythmPhase.OperationAccess,
                    InteractiveType = InteractiveType.Terminal,
                }),

            MakeZone(HubZoneId.ObservationGallery, "Обзорная галерея",
                0f, 0f, 10f, 8f, 3f, 2f,
                new InteractableObject
                {
                    Id = "gallery_overlook",
                    DisplayName = "Обзорная точка",
                    HintText = "Посмотри вниз",
                    Zone = HubZoneId.ObservationGallery,
                    Position = new Vec3(0f, 0f, 10f),
                    Bounds = new Bounds3D(-0.5f, 0f, 9.5f, 1f, 2f, 1f),
                    InteractionRadius = 3f,
                    RequiredPhase = HubRhythmPhase.OperationAccess,
                    InteractiveType = InteractiveType.Trigger,
                    DialogueId = "DIA_GALLERY_OVERLOOK",
                }),

            MakeZone(HubZoneId.HardArchiveEntrance, "Вход в Хард-Архив",
                0f, -1f, 11f, 4f, 5f, 2f,
                new InteractableObject
                {
                    Id = "archive_gate",
                    DisplayName = "Врата Хард-Архива",
                    HintText = "Доступ заблокирован",
                    Zone = HubZoneId.HardArchiveEntrance,
                    Position = new Vec3(0f, 0f, 11f),
                    Bounds = new Bounds3D(-1f, -1f, 10.5f, 2f, 4f, 1f),
                    InteractionRadius = 2f,
                    RequiredPhase = HubRhythmPhase.OperationAccess,
                    InteractiveType = InteractiveType.Gate,
                    DialogueId = "DIA_ARCHIVE_LOCKED",
                    LockedMessage = "Врата Хард-Архива заблокированы.",
                }),

            // Locked gates — no objects
            new HubZone
            {
                Id = HubZoneId.CommerceGateLocked,
                Name = "Коммерческий шлюз",
                Position = new Vec3(-6f, 0f, 6f),
                Bounds = new Bounds3D(-7.5f, 0f, 4.5f, 3f, 3f, 3f),
                Objects = [],
            },
            new HubZone
            {
                Id = HubZoneId.TechGateLocked,
                Name = "Технический шлюз",
                Position = new Vec3(0f, 0f, -8f),
                Bounds = new Bounds3D(-1.5f, 0f, -9.5f, 3f, 3f, 3f),
                Objects = [],
            },
        ];
    }

    private static HubZone MakeZone(
        HubZoneId id, string name,
        float x, float y, float z,
        float w, float h, float d,
        params InteractableObject[] objects)
    {
        // Bounds centred on position
        return new HubZone
        {
            Id = id,
            Name = name,
            Position = new Vec3(x, y, z),
            Bounds = new Bounds3D(x - w / 2f, y, z - d / 2f, w, h, d),
            Objects = [.. objects],
        };
    }
}
