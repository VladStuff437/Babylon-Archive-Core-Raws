using BabylonArchiveCore.Content.DataContracts;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Content.Pipeline;

/// <summary>
/// Maps JSON DataContracts to Domain models for scene A-0.
/// </summary>
public static class A0ContentMapper
{
    private static readonly Dictionary<string, HubZoneId> ZoneIdMap = new()
    {
        ["ZONE_CAPSULE"] = HubZoneId.Capsule,
        ["ZONE_BIOMETRIC"] = HubZoneId.Biometrics,
        ["ZONE_LOGISTICS"] = HubZoneId.Logistics,
        ["ZONE_DRONE_NICHE"] = HubZoneId.DroneNiche,
        ["ZONE_CORE"] = HubZoneId.Core,
        ["ZONE_MISSION_TERMINAL"] = HubZoneId.MissionTerminal,
        ["ZONE_RESEARCH"] = HubZoneId.Research,
        ["ZONE_OBSERVATION_GALLERY"] = HubZoneId.ObservationGallery,
        ["ZONE_HARD_ARCHIVE_ENTRY_LOCKED"] = HubZoneId.HardArchiveEntrance,
        ["ZONE_COMMERCE_GATE_LOCKED"] = HubZoneId.CommerceGateLocked,
        ["ZONE_TECH_GATE_LOCKED"] = HubZoneId.TechGateLocked,
    };

    private static readonly Dictionary<string, HubRhythmPhase> PhaseMap = new()
    {
        ["Awakening"] = HubRhythmPhase.Awakening,
        ["Identification"] = HubRhythmPhase.Identification,
        ["Provisioning"] = HubRhythmPhase.Provisioning,
        ["DroneContact"] = HubRhythmPhase.DroneContact,
        ["Activation"] = HubRhythmPhase.Activation,
        ["OperationAccess"] = HubRhythmPhase.OperationAccess,
    };

    private static readonly Dictionary<string, InteractiveType> TypeMap = new()
    {
        ["trigger"] = InteractiveType.Trigger,
        ["terminal"] = InteractiveType.Terminal,
        ["npc"] = InteractiveType.Npc,
        ["gate"] = InteractiveType.Gate,
    };

    private static readonly Dictionary<string, string> ObjectIdMap = new()
    {
        ["OBJ_CAPSULE_EXIT"] = "capsule_exit",
        ["OBJ_BIO_SCANNER"] = "bio_scanner",
        ["OBJ_SUPPLY_TERMINAL"] = "supply_terminal",
        ["OBJ_DRONE_DOCK"] = "drone_dock",
        ["OBJ_CORE_CONSOLE"] = "core_console",
        ["OBJ_OP_TERMINAL"] = "op_terminal",
        ["OBJ_RESEARCH_TERMINAL"] = "research_terminal",
        ["OBJ_GALLERY_OVERLOOK"] = "gallery_overlook",
        ["OBJ_ARCHIVE_GATE"] = "archive_gate",
    };

    public static HubZoneId MapZoneId(string zoneId) =>
        ZoneIdMap.TryGetValue(zoneId, out var z)
            ? z
            : throw new ArgumentException($"Unknown zone ID: {zoneId}");

    public static HubRhythmPhase MapPhase(string phase) =>
        PhaseMap.TryGetValue(phase, out var p)
            ? p
            : throw new ArgumentException($"Unknown phase: {phase}");

    public static InteractiveType MapType(string type) =>
        TypeMap.TryGetValue(type, out var t)
            ? t
            : throw new ArgumentException($"Unknown interactive type: {type}");

    public static string MapObjectId(string jsonObjectId) =>
        ObjectIdMap.TryGetValue(jsonObjectId, out var id)
            ? id
            : throw new ArgumentException($"Unknown object ID: {jsonObjectId}");

    public static Vec3 MapVec3(Vec3Data v) => new(v.X, v.Y, v.Z);

    public static List<HubZone> MapZones(ZoneFileData data, ObjectFileData objectData)
    {
        var objectsByZone = objectData.Objects
            .GroupBy(o => o.ZoneId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var zones = new List<HubZone>();

        foreach (var z in data.Zones)
        {
            var zoneId = MapZoneId(z.ZoneId);
            var pos = MapVec3(z.Position);
            var objects = new List<InteractableObject>();

            if (objectsByZone.TryGetValue(z.ZoneId, out var zoneObjects))
            {
                foreach (var obj in zoneObjects)
                    objects.Add(MapObject(obj, zoneId));
            }

            zones.Add(new HubZone
            {
                Id = zoneId,
                Name = z.DisplayName,
                Position = pos,
                Bounds = new Bounds3D(
                    pos.X - z.Size.Width / 2f, pos.Y, pos.Z - z.Size.Depth / 2f,
                    z.Size.Width, z.Size.Height, z.Size.Depth),
                Objects = objects,
            });
        }

        return zones;
    }

    public static InteractableObject MapObject(InteractableObjectData data, HubZoneId zone)
    {
        var pos = MapVec3(data.Position);
        return new InteractableObject
        {
            Id = MapObjectId(data.ObjectId),
            DisplayName = data.DisplayName,
            HintText = data.HintText,
            Zone = zone,
            Position = pos,
            Bounds = new Bounds3D(pos.X - 0.5f, pos.Y, pos.Z - 0.5f, 1f, 2f, 1f),
            InteractionRadius = data.InteractionRadius,
            RequiredPhase = MapPhase(data.RequiredPhase),
            InteractiveType = MapType(data.Type),
            DialogueId = data.DialogueId,
            GrantsItems = data.GrantsItems,
            LockedMessage = data.LockedMessage,
            ModelId = data.ModelId,
        };
    }

    public static List<InlineDialogue> MapDialogues(DialogueFileData data)
    {
        var dialogues = new List<InlineDialogue>();
        foreach (var d in data.Dialogues)
        {
            dialogues.Add(new InlineDialogue
            {
                DialogueId = d.DialogueId,
                SpeakerDisplayName = d.SpeakerDisplayName,
                Lines = d.Lines.Select(l => new InlineDialogueLine
                {
                    LineId = l.LineId,
                    Speaker = d.Speaker,
                    Text = l.Text,
                    Delay = l.Delay,
                }).ToList(),
            });
        }
        return dialogues;
    }

    public static List<PhaseTrigger> MapTriggers(TriggerFileData data)
    {
        var triggers = new List<PhaseTrigger>();

        foreach (var t in data.Triggers)
        {
            if (t.Type != "phase_change") continue;

            var phaseName = ExtractPhaseFromCondition(t.Condition);
            if (phaseName is null || !PhaseMap.TryGetValue(phaseName, out var phase))
                continue;

            var actions = t.Actions
                .Select(MapTriggerAction)
                .Where(a => a is not null)
                .Cast<TriggerAction>()
                .ToList();

            triggers.Add(new PhaseTrigger
            {
                TriggerId = t.TriggerId,
                Phase = phase,
                Actions = actions,
            });
        }

        return triggers;
    }

    public static IReadOnlyDictionary<string, IReadOnlyList<TriggerAction>> MapObjectInteractionTriggers(TriggerFileData data)
    {
        var result = new Dictionary<string, IReadOnlyList<TriggerAction>>(StringComparer.OrdinalIgnoreCase);

        foreach (var t in data.Triggers)
        {
            if (!string.Equals(t.Type, "object_interact", StringComparison.OrdinalIgnoreCase))
                continue;

            var objectJsonId = ExtractObjectIdFromCondition(t.Condition);
            if (objectJsonId is null)
                continue;

            var mappedId = MapObjectId(objectJsonId);
            var actions = t.Actions
                .Select(MapTriggerAction)
                .Where(a => a is not null)
                .Cast<TriggerAction>()
                .ToList();

            result[mappedId] = actions;
        }

        return result;
    }

    public static List<Objective> MapObjectives(ObjectiveFileData data)
    {
        return data.Objectives
            .Select(o => new Objective
            {
                ObjectiveId = o.ObjectiveId,
                Text = o.Text,
                Order = o.Order,
            })
            .ToList();
    }

    private static TriggerAction? MapTriggerAction(TriggerActionData a) =>
        a.Type switch
        {
            "set_objective" => new TriggerAction
            {
                Type = TriggerActionType.SetObjective,
                ObjectiveId = a.ObjectiveId,
            },
            "play_dialogue" when a.DialogueId is not null => new TriggerAction
            {
                Type = TriggerActionType.PlayDialogue,
                DialogueId = a.DialogueId,
            },
            "journal_entry" => new TriggerAction
            {
                Type = TriggerActionType.JournalEntry,
                Text = a.Text,
            },
            _ => null,
        };

    private static string? ExtractPhaseFromCondition(string condition)
    {
        // Format: "phase == PhaseName"
        var parts = condition.Split("==", StringSplitOptions.TrimEntries);
        return parts.Length == 2 && parts[0] == "phase" ? parts[1] : null;
    }

    private static string? ExtractObjectIdFromCondition(string condition)
    {
        // Format: "objectId == OBJ_X"
        var parts = condition.Split("==", StringSplitOptions.TrimEntries);
        return parts.Length == 2 && parts[0] == "objectId" ? parts[1] : null;
    }
}
