namespace BabylonArchiveCore.Content.DataContracts;

/// <summary>
/// Matches Content/Zones/A0_Zones.json
/// </summary>
public sealed class ZoneFileData
{
    public string SceneId { get; set; } = "";
    public List<ZoneData> Zones { get; set; } = [];
}

public sealed class ZoneData
{
    public string ZoneId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public Vec3Data Position { get; set; } = new(0, 0, 0);
    public SizeData Size { get; set; } = new(0, 0, 0);
    public string RequiredPhase { get; set; } = "";
    public bool IsLocked { get; set; }
}
