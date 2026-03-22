namespace BabylonArchiveCore.Content.DataContracts;

/// <summary>
/// Matches Content/Objects/A0_Objects.json
/// </summary>
public sealed class ObjectFileData
{
    public string SceneId { get; set; } = "";
    public List<InteractableObjectData> Objects { get; set; } = [];
}

public sealed class InteractableObjectData
{
    public string ObjectId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string ZoneId { get; set; } = "";
    public string Type { get; set; } = "";
    public Vec3Data Position { get; set; } = new(0, 0, 0);
    public float InteractionRadius { get; set; } = 1.5f;
    public string HintText { get; set; } = "";
    public string RequiredPhase { get; set; } = "";
    public bool AdvancesPhase { get; set; }
    public string? NextPhase { get; set; }
    public string? DialogueId { get; set; }
    public string? LockedMessage { get; set; }
    public List<string>? GrantsItems { get; set; }
}
