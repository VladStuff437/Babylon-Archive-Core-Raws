namespace BabylonArchiveCore.Content.DataContracts;

/// <summary>
/// Matches Content/Dialogue/A0_Dialogue.json
/// </summary>
public sealed class DialogueFileData
{
    public string SceneId { get; set; } = "";
    public List<DialogueData> Dialogues { get; set; } = [];
}

public sealed class DialogueData
{
    public string DialogueId { get; set; } = "";
    public string Speaker { get; set; } = "";
    public string SpeakerDisplayName { get; set; } = "";
    public List<DialogueLineData> Lines { get; set; } = [];
    public List<DialogueOptionData> Options { get; set; } = [];
}

public sealed class DialogueLineData
{
    public string LineId { get; set; } = "";
    public string Text { get; set; } = "";
    public float Delay { get; set; } = 1.5f;
}

public sealed class DialogueOptionData
{
    public string OptionId { get; set; } = "";
    public string Text { get; set; } = "";
    public string? RequiredStat { get; set; }
    public int? RequiredStatValue { get; set; }
    public string? NextDialogueId { get; set; }
}
