namespace BabylonArchiveCore.Content.DataContracts;

/// <summary>
/// Matches Content/UI/A0_Objectives.json
/// </summary>
public sealed class ObjectiveFileData
{
    public string SceneId { get; set; } = "";
    public List<ObjectiveData> Objectives { get; set; } = [];
}

public sealed class ObjectiveData
{
    public string ObjectiveId { get; set; } = "";
    public string Text { get; set; } = "";
    public string Phase { get; set; } = "";
    public string? TargetObject { get; set; }
    public int Order { get; set; }
}
