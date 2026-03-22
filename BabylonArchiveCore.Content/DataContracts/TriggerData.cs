namespace BabylonArchiveCore.Content.DataContracts;

/// <summary>
/// Matches Content/Triggers/A0_Triggers.json
/// </summary>
public sealed class TriggerFileData
{
    public string SceneId { get; set; } = "";
    public List<TriggerData> Triggers { get; set; } = [];
}

public sealed class TriggerData
{
    public string TriggerId { get; set; } = "";
    public string Type { get; set; } = "";
    public string Condition { get; set; } = "";
    public List<TriggerActionData> Actions { get; set; } = [];
}

public sealed class TriggerActionData
{
    public string Type { get; set; } = "";
    public string? ObjectiveId { get; set; }
    public string? DialogueId { get; set; }
    public string? CameraId { get; set; }
    public string? MissionId { get; set; }
    public string? Text { get; set; }
}
