namespace BabylonArchiveCore.Content.DataContracts;

/// <summary>
/// Matches Content/Scenes/SCN_*.json
/// </summary>
public sealed class SceneData
{
    public string SceneId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public Vec3Data WorldOrigin { get; set; } = new(0, 0, 0);
    public SizeData Dimensions { get; set; } = new(0, 0, 0);
    public string Shape { get; set; } = "octagonal";
    public string AmbientLightColor { get; set; } = "#1a2235";
    public float AmbientLightIntensity { get; set; } = 0.3f;
    public string StartingZone { get; set; } = "";
    public string StartingPhase { get; set; } = "";
    public List<CameraProfileData> CameraProfiles { get; set; } = [];
}

public sealed class CameraProfileData
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "third_person";
    public float Fov { get; set; } = 60f;
    public float MinDistance { get; set; }
    public float MaxDistance { get; set; }
    public float PitchMin { get; set; }
    public float PitchMax { get; set; }
    public float? FixedPitch { get; set; }
    public float? FixedYaw { get; set; }
    public float? Distance { get; set; }
}
