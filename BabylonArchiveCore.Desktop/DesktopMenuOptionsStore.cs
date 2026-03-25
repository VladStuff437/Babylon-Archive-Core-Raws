using System.Text.Json;
using BabylonArchiveCore.Runtime.Control;
using System.Windows.Forms;

namespace BabylonArchiveCore.Desktop;

public sealed class DesktopControlProfileEntry
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool IsCustom { get; init; }
    public ControlV2Profile Profile { get; init; } = ControlV2Profile.Default;
}

public sealed class DesktopMenuOptions
{
    public int SchemaVersion { get; init; } = 3;
    public string ActiveControlProfileId { get; init; } = ControlProfilePresetCatalog.ModernThirdPersonId;
    public string DefaultControlProfileId { get; init; } = ControlProfilePresetCatalog.ModernThirdPersonId;
    public List<DesktopControlProfileEntry> ControlProfiles { get; init; } = [];
    public DesktopInputBindings InputBindings { get; init; } = DesktopInputBindings.Default;

    // Legacy flat control values retained for migration from previous schema.
    public bool InvertOrbitHorizontal { get; init; }
    public bool InvertOrbitVertical { get; init; }
    public float OrbitSensitivity { get; init; } = 0.0075f;
    public float OrbitSensitivityVertical { get; init; } = 0.0040f;
    public float CameraSmoothing { get; init; } = 7.5f;
    public float MaxOrbitOffsetYaw { get; init; } = 1.05f;
    public float MaxOrbitVerticalOffset { get; init; } = 1.8f;
    public bool FxCapsuleSteamEnabled { get; init; } = true;
    public bool FxCoreHologramEnabled { get; init; } = true;
    public bool FxArchiveRouteGuideEnabled { get; init; } = true;
    public bool ShowExtendedHudTelemetry { get; init; } = true;
    public bool UnlockAllRooms { get; init; }
    public bool UnlockAllTerminals { get; init; }
    public bool UnlockHardArchivePreview { get; init; }
    public bool EnablePurchaseSimulation { get; init; }
    public bool EnableMissionPageDebugAccess { get; init; }
}

public sealed record DesktopInputBindings
{
    public Keys MoveForward { get; init; } = Keys.W;
    public Keys MoveBackward { get; init; } = Keys.S;
    public Keys MoveLeft { get; init; } = Keys.A;
    public Keys MoveRight { get; init; } = Keys.D;
    public Keys TurnLeft { get; init; } = Keys.A;
    public Keys TurnRight { get; init; } = Keys.D;
    public Keys Interact { get; init; } = Keys.E;
    public Keys CameraToggle { get; init; } = Keys.Q;
    public Keys OrbitModifier { get; init; } = Keys.None;
    public Keys DialogueAdvance { get; init; } = Keys.Space;
    public Keys PauseMenu { get; init; } = Keys.Escape;
    public Keys ResetSession { get; init; } = Keys.R;

    public static DesktopInputBindings Default => new();
}

public sealed class DesktopMenuOptionsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public DesktopMenuOptions LoadOrCreate(string filePath, Func<DesktopMenuOptions> defaultsFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(defaultsFactory);

        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            var existing = JsonSerializer.Deserialize<DesktopMenuOptions>(content, JsonOptions);
            if (existing is not null)
                return existing;
        }

        var defaults = defaultsFactory();
        Save(filePath, defaults);
        return defaults;
    }

    public void Save(string filePath, DesktopMenuOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(options);

        var json = JsonSerializer.Serialize(options, JsonOptions);
        File.WriteAllText(filePath, json);
    }
}
