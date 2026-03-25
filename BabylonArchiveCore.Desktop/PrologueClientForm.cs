using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Input;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Domain.World.Runtime;
using BabylonArchiveCore.Infrastructure.Logging;
using BabylonArchiveCore.Infrastructure.Save;
using BabylonArchiveCore.Runtime.Control;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.Mission;
using BabylonArchiveCore.Runtime.Save;
using BabylonArchiveCore.Runtime.Scene;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text;

namespace BabylonArchiveCore.Desktop;

public sealed class PrologueClientForm : Form
{
    private sealed class RenderViewportPanel : Panel
    {
        public RenderViewportPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
    }

    private sealed record ExtraInteractionNode(string Id, string DisplayName, Vec3 Position, float Radius, string? ModelId = null);
    private sealed record EnvModule(string Id, Vec3 Position, Vec3 Size, Color Color, bool Emissive = false);
    private sealed record MenuHitTarget(Rectangle Bounds, Action OnClick, bool IsEnabled = true);

    private enum EscMenuPage
    {
        Root,
        Options,
    }

    private enum OptionsCategory
    {
        Controls,
        Camera,
        Mouse,
        Bindings,
        Graphics,
        Audio,
        Ui,
        Gameplay,
        Admin,
    }

    private static readonly ExtraInteractionNode[] ExtraNodes =
    [
        new("commerce_hall", "Коммерческий зал", new Vec3(-6.0f, 0.0f, 6.0f), 2.2f, ContourModelIds.CommerceHall),
        new("tech_hall", "Технический зал", new Vec3(0.0f, 0.0f, -8.0f), 2.2f, ContourModelIds.TechHall),
        new("archive_preview", "Контур Хард-Архива", new Vec3(0.0f, 0.0f, -15.0f), 2.5f, ContourModelIds.ArchivePreview),
        new("mission_board", "Доска операций", new Vec3(0.0f, 0.0f, 8.0f), 2.0f, ContourModelIds.MissionBoard),
        new("research_lab", "Аналитический узел", new Vec3(6.0f, 0.0f, 6.0f), 2.0f, ContourModelIds.ResearchLab),
        new("archive_control", "Контур контроля архива", new Vec3(0.0f, 0.0f, 11.5f), 2.0f, ContourModelIds.ArchiveControl),
        new("tool_bench", "Сборочный стол", new Vec3(2.5f, 0.0f, -9.5f), 1.8f, ContourModelIds.ToolBench),
        new("commerce_desk", "Сервисный модуль", new Vec3(-8.6f, 0.0f, 5.3f), 1.8f, ContourModelIds.CommerceDesk),
        new("archive_corridor", "Коридор доступа", new Vec3(0.0f, 0.0f, 14.8f), 2.2f, ContourModelIds.ArchiveCorridor),
        new("entry_octagon", "Entry Octagon", new Vec3(0.0f, 0.0f, 18.2f), 2.4f, ContourModelIds.EntryOctagon),
        new("index_vestibule", "Index Vestibule", new Vec3(0.0f, 0.0f, 22.4f), 2.4f, ContourModelIds.IndexVestibule),
        new("research_room_01", "Research Room 01", new Vec3(4.8f, 0.0f, 26.4f), 2.2f, ContourModelIds.ResearchRoom01),
        new("stack_ring_preview", "Stack Ring Preview", new Vec3(0.0f, -1.4f, 30.6f), 3.0f, ContourModelIds.StackRingPreview),
    ];

    private static readonly EnvModule[] EnvironmentModules =
    [
        new("WALL_N", new Vec3(0.0f, 3.2f, -16.5f), new Vec3(22.0f, 6.2f, 0.6f), Color.FromArgb(45, 56, 72)),
        new("WALL_S", new Vec3(0.0f, 3.2f, 10.4f), new Vec3(18.0f, 6.2f, 0.6f), Color.FromArgb(45, 56, 72)),
        new("WALL_W", new Vec3(-14.4f, 3.2f, -3.2f), new Vec3(0.6f, 6.2f, 26.0f), Color.FromArgb(42, 53, 68)),
        new("WALL_E", new Vec3(14.4f, 3.2f, -3.2f), new Vec3(0.6f, 6.2f, 26.0f), Color.FromArgb(42, 53, 68)),
        new("CORE_RING_1", new Vec3(0.0f, 0.15f, 0.0f), new Vec3(8.0f, 0.25f, 8.0f), Color.FromArgb(58, 76, 102), true),
        new("CORE_RING_2", new Vec3(0.0f, 0.1f, 0.0f), new Vec3(10.0f, 0.12f, 10.0f), Color.FromArgb(38, 58, 84)),
        new("GALLERY_RAIL_L", new Vec3(-3.9f, 1.1f, -11.0f), new Vec3(0.2f, 1.6f, 3.2f), Color.FromArgb(150, 166, 182)),
        new("GALLERY_RAIL_R", new Vec3(3.9f, 1.1f, -11.0f), new Vec3(0.2f, 1.6f, 3.2f), Color.FromArgb(150, 166, 182)),
        new("SHAFT_L1", new Vec3(0.0f, -1.8f, -18.0f), new Vec3(6.0f, 0.2f, 6.0f), Color.FromArgb(44, 58, 80)),
        new("SHAFT_L2", new Vec3(0.0f, -4.4f, -20.2f), new Vec3(8.0f, 0.2f, 8.0f), Color.FromArgb(35, 48, 70)),
        new("SHAFT_L3", new Vec3(0.0f, -7.0f, -22.4f), new Vec3(10.0f, 0.2f, 10.0f), Color.FromArgb(28, 40, 60)),
        new("MISSION_WALL", new Vec3(0.0f, 1.8f, 8.8f), new Vec3(5.2f, 3.6f, 0.5f), Color.FromArgb(76, 74, 52)),
        new("RESEARCH_WALL", new Vec3(6.5f, 1.8f, 6.8f), new Vec3(3.2f, 3.6f, 0.5f), Color.FromArgb(54, 72, 96)),
        new("COMMERCE_GATE", new Vec3(-6.5f, 1.6f, -9.0f), new Vec3(2.8f, 3.2f, 0.8f), Color.FromArgb(120, 76, 70), true),
        new("TECH_GATE", new Vec3(6.5f, 1.6f, -9.0f), new Vec3(2.8f, 3.2f, 0.8f), Color.FromArgb(72, 92, 126), true),
        new("ARCHIVE_CTRL_COL", new Vec3(0.0f, 1.6f, 12.4f), new Vec3(1.2f, 3.2f, 1.2f), Color.FromArgb(134, 92, 84), true),
        new("RING_PILLAR_NW", new Vec3(-5.6f, 1.8f, -4.8f), new Vec3(0.8f, 3.6f, 0.8f), Color.FromArgb(102, 118, 140)),
        new("RING_PILLAR_NE", new Vec3(5.6f, 1.8f, -4.8f), new Vec3(0.8f, 3.6f, 0.8f), Color.FromArgb(102, 118, 140)),
        new("RING_PILLAR_SW", new Vec3(-5.6f, 1.8f, 4.8f), new Vec3(0.8f, 3.6f, 0.8f), Color.FromArgb(102, 118, 140)),
        new("RING_PILLAR_SE", new Vec3(5.6f, 1.8f, 4.8f), new Vec3(0.8f, 3.6f, 0.8f), Color.FromArgb(102, 118, 140)),
        new("MID_BRIDGE_N", new Vec3(0.0f, 0.95f, -6.4f), new Vec3(7.8f, 0.3f, 1.0f), Color.FromArgb(78, 96, 124)),
        new("MID_BRIDGE_S", new Vec3(0.0f, 0.95f, 6.4f), new Vec3(7.8f, 0.3f, 1.0f), Color.FromArgb(78, 96, 124)),
        new("ARCH_ELEV_N", new Vec3(0.0f, 3.5f, -10.8f), new Vec3(10.4f, 0.45f, 0.7f), Color.FromArgb(88, 104, 130), true),
        new("ARCH_ELEV_S", new Vec3(0.0f, 3.5f, 8.8f), new Vec3(10.4f, 0.45f, 0.7f), Color.FromArgb(88, 104, 130), true),
        new("OBS_WALKWAY_L", new Vec3(-8.8f, 0.65f, -2.2f), new Vec3(1.2f, 0.24f, 9.6f), Color.FromArgb(64, 84, 110)),
        new("OBS_WALKWAY_R", new Vec3(8.8f, 0.65f, -2.2f), new Vec3(1.2f, 0.24f, 9.6f), Color.FromArgb(64, 84, 110)),
        new("CEIL_TRUSS_N", new Vec3(0.0f, 5.2f, -6.5f), new Vec3(12.4f, 0.24f, 0.5f), Color.FromArgb(92, 106, 124)),
        new("CEIL_TRUSS_C", new Vec3(0.0f, 5.3f, -1.8f), new Vec3(12.8f, 0.24f, 0.5f), Color.FromArgb(86, 100, 118)),
        new("CEIL_TRUSS_S", new Vec3(0.0f, 5.2f, 2.8f), new Vec3(12.4f, 0.24f, 0.5f), Color.FromArgb(92, 106, 124)),
        new("BULKHEAD_L", new Vec3(-11.2f, 2.0f, -8.8f), new Vec3(0.6f, 4.0f, 2.8f), Color.FromArgb(72, 90, 116)),
        new("BULKHEAD_R", new Vec3(11.2f, 2.0f, -8.8f), new Vec3(0.6f, 4.0f, 2.8f), Color.FromArgb(72, 90, 116)),
        new("AUX_RING", new Vec3(0.0f, 0.08f, 0.0f), new Vec3(12.8f, 0.08f, 12.8f), Color.FromArgb(30, 44, 66)),
        new("ARCHIVE_CORRIDOR_FLOOR", new Vec3(0.0f, -0.05f, 15.8f), new Vec3(4.2f, 0.1f, 6.2f), Color.FromArgb(52, 70, 92)),
        new("ARCHIVE_CORRIDOR_L", new Vec3(-2.2f, 1.5f, 15.8f), new Vec3(0.3f, 3.0f, 6.2f), Color.FromArgb(66, 86, 116)),
        new("ARCHIVE_CORRIDOR_R", new Vec3(2.2f, 1.5f, 15.8f), new Vec3(0.3f, 3.0f, 6.2f), Color.FromArgb(66, 86, 116)),
        new("ENTRY_OCTAGON_RING", new Vec3(0.0f, 0.1f, 18.8f), new Vec3(7.4f, 0.2f, 7.4f), Color.FromArgb(74, 96, 124), true),
        new("ENTRY_OCTAGON_CORE", new Vec3(0.0f, 0.95f, 18.8f), new Vec3(1.0f, 1.8f, 1.0f), Color.FromArgb(112, 170, 214), true),
        new("VESTIBULE_PATH", new Vec3(0.0f, 0.06f, 22.8f), new Vec3(3.6f, 0.12f, 4.8f), Color.FromArgb(58, 78, 106)),
        new("VESTIBULE_NODE_L", new Vec3(-1.6f, 1.0f, 22.8f), new Vec3(0.7f, 2.0f, 0.7f), Color.FromArgb(116, 146, 178)),
        new("VESTIBULE_NODE_R", new Vec3(1.6f, 1.0f, 22.8f), new Vec3(0.7f, 2.0f, 0.7f), Color.FromArgb(116, 146, 178)),
        new("RESEARCH_ROOM_BLOCK", new Vec3(4.8f, 1.2f, 26.4f), new Vec3(5.2f, 2.4f, 4.8f), Color.FromArgb(64, 88, 120)),
        new("RESEARCH_ROOM_CONSOLE", new Vec3(4.8f, 1.0f, 25.2f), new Vec3(1.1f, 2.0f, 0.8f), Color.FromArgb(112, 196, 228), true),
        new("STACK_RING_FLOOR", new Vec3(0.0f, -1.7f, 30.8f), new Vec3(14.8f, 0.16f, 14.8f), Color.FromArgb(44, 62, 90)),
        new("STACK_RING_VOID_MARK", new Vec3(0.0f, -1.6f, 30.8f), new Vec3(5.2f, 0.12f, 5.2f), Color.FromArgb(84, 124, 176), true),
    ];

    private readonly WorldRuntimeProfile _runtimeProfile;
    private readonly RenderViewportPanel _viewport = new() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(10, 12, 18) };
    private readonly RichTextBox _logBox = new() { ReadOnly = true, Dock = DockStyle.Fill, Font = new Font("Consolas", 9f) };
    private readonly TextBox _noteInput = new() { Dock = DockStyle.Fill, Font = new Font("Consolas", 9f), PlaceholderText = "Введите заметку для Copilot и нажмите Enter или кнопку Сохранить" };
    private readonly Button _saveNoteButton = new() { Text = "Сохранить", Dock = DockStyle.Fill, Height = 30 };
    private readonly System.Windows.Forms.Timer _loopTimer = new() { Interval = 16 };
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private long _lastTicks;

    private readonly HashSet<Keys> _keysDown = [];
    private readonly HashSet<Keys> _keysPressed = [];
    private bool _leftMouseDown;
    private bool _middleMouseDown;
    private bool _rightMouseDown;
    private bool _mousePositionInitialized;
    private float _mouseDeltaX;
    private float _mouseDeltaY;
    private Point _lastMousePos;
    private readonly SaveGameStore _saveGameStore = new();
    private readonly DesktopMenuOptionsStore _menuOptionsStore = new();
    private const int DesktopSaveVersion = 2;
    private const string DesktopSaveFileName = "savegame.desktop.json";
    private const string DesktopOptionsFileName = "desktop-options.json";

    private readonly ControllerState _controllerState = new();
    private readonly List<DesktopControlProfileEntry> _controlProfiles = [];
    private string _activeControlProfileId = ControlProfilePresetCatalog.ModernThirdPersonId;
    private string _defaultControlProfileId = ControlProfilePresetCatalog.ModernThirdPersonId;
    private int _customProfileCounter = 1;
    private ControlV2Profile _controlProfile = ControlProfilePresetCatalog.GetBuiltInProfileOrDefault(ControlProfilePresetCatalog.ModernThirdPersonId);
    private DesktopInputBindings _inputBindings = DesktopInputBindings.Default;
    private float _fps;
    private int _framesInWindow;
    private long _fpsWindowStart;

    private ContentDrivenSceneFactory.SceneBundle? _bundle;
    private HintSystem? _hints;
    private ObjectiveHUD? _objectiveHud;

    private string? _focusedObjectId;
    private string? _focusedObjectName;
    private string? _focusedExtraId;
    private string? _focusedExtraName;
    private string? _statusMessage;
    private TerminalScreen? _activeTerminal;
    private bool _protocolObjectiveApplied;
    private IReadOnlyList<TerminalMissionBoardSlot> _missionSlots = [];
    private IReadOnlyList<string> _archivePreviewRooms = [];
    private string? _sessionLogPath;
    private string? _sessionCurrentLogPath;
    private string? _notesLogPath;
    private HubRhythmPhase _lastLoggedPhase = HubRhythmPhase.Awakening;
    private string? _lastLoggedObjectiveId;
    private string? _lastLoggedFocus;
    private string? _lastLoggedTerminalId;
    private CameraMode _lastLoggedCameraMode = CameraMode.ThirdPerson3D;
    private bool _lastLoggedProtocolZero;
    private string? _lastLoggedStatusMessage;
    private long _lastHeartbeatMs;
    private string? _lastImperativeKey;
    private string _lastSceneTitle = string.Empty;
    private Vec3 _lastHeartbeatPos;
    private Vec3 _lastPlayerPos;
    private double _blockedMovementMs;
    private bool _isPathBlocked;
    private long _lastIntegrationProbeMs;
    private long _lastRenderProbeMs;
    private string? _lastRenderSummary;
    private long _lastRenderPerfProbeMs;
    private bool _fxCapsuleSteamEnabled = true;
    private bool _fxCoreHologramEnabled = true;
    private bool _fxArchiveRouteGuideEnabled = true;
    private double _lastDrawMs;
    private double _drawMsAverage;
    private double _drawMsAccumulator;
    private int _drawMsSamples;
    private int _lastVisibleInteractables;
    private int _lastVisibleExtraNodes;
    private int _lastArchiveChainCompleted = -1;
    private string? _lastArchiveChainNextNode;
    private int _lastCheckpointPhaseRank;
    private bool _operationQuestionIssued;
    private bool _actOneFlowQuestionIssued;
    private bool _actOneCompleteQuestionIssued;
    private bool _uiInputCaptureActive;
    private bool _showExtendedHudTelemetry = true;
    private bool _adminUnlockAllRooms;
    private bool _adminUnlockAllTerminals;
    private bool _adminUnlockHardArchivePreview;
    private bool _adminEnablePurchaseSimulation;
    private bool _adminEnableMissionPageDebugAccess;
    private bool _pauseMenuOpen;
    private EscMenuPage _escMenuPage = EscMenuPage.Root;
    private OptionsCategory _activeOptionsCategory = OptionsCategory.Controls;
    private int _activeOptionsPage;
    private readonly List<MenuHitTarget> _menuHitTargets = [];
    private readonly List<string> _menuToastLines = [];
    private long _menuToastExpiresAtMs;
    private string? _saveGamePath;
    private string? _optionsPath;
    private bool _hardArchivePartialUnlock;
    private bool _entryOctagonUnlocked;
    private bool _indexVestibuleUnlocked;
    private bool _researchRoomUnlocked;
    private bool _stackRingPreviewUnlocked;

    private static readonly Dictionary<string, (Vec3 Pos, Vec3 Size, Color Color)> WhiteboxStatics = new()
    {
        ["CAPSULE"] = (new Vec3(-8.0f, 1.1f, 6.0f), new Vec3(2.2f, 2.2f, 4.2f), Color.FromArgb(80, 160, 255)),
        ["CORE_PLATFORM"] = (new Vec3(0.0f, 0.2f, 0.0f), new Vec3(4.0f, 0.4f, 4.0f), Color.FromArgb(130, 130, 140)),
        ["OBS_GALLERY"] = (new Vec3(0.0f, 0.1f, -11.0f), new Vec3(8.0f, 0.2f, 3.0f), Color.FromArgb(170, 170, 185)),
        ["HA_GATE_LOCKED"] = (new Vec3(0.0f, 2.2f, -15.0f), new Vec3(4.5f, 4.5f, 1.2f), Color.FromArgb(220, 90, 90)),
        ["COMMERCE_LOCKED"] = (new Vec3(-6.5f, 1.6f, -9.0f), new Vec3(2.8f, 3.2f, 0.8f), Color.FromArgb(170, 80, 80)),
        ["TECH_LOCKED"] = (new Vec3(6.5f, 1.6f, -9.0f), new Vec3(2.8f, 3.2f, 0.8f), Color.FromArgb(170, 80, 80)),
    };

    public PrologueClientForm(WorldRuntimeProfile runtimeProfile)
    {
        _runtimeProfile = runtimeProfile;
        Text = "Babylon Archive Core - Session 10 Gameplay Harness";
        Width = 1400;
        Height = 880;
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true;

        BuildUi();
        InitializeRuntime();

        _loopTimer.Tick += (_, _) => TickFrame();
        _loopTimer.Start();

        _lastTicks = _clock.ElapsedTicks;
        _fpsWindowStart = _clock.ElapsedMilliseconds;
    }

    private bool IsAdminMode => _runtimeProfile.AccessMode == WorldAccessMode.Admin;

    private bool UnlockAllRoomsEnabled => _adminUnlockAllRooms;

    private bool UnlockAllTerminalsEnabled => _adminUnlockAllTerminals;

    private bool UnlockHardArchivePreviewEnabled => _adminUnlockHardArchivePreview;

    private bool PurchaseSimulationEnabled => _adminEnablePurchaseSimulation;

    private bool MissionPageDebugAccessEnabled => _adminEnableMissionPageDebugAccess;

    private bool StrictUiInputLockEnabled => true;

    private bool IsPauseMenuOpen => _pauseMenuOpen;

    private void BuildUi()
    {
        var root = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 980,
        };
        Controls.Add(root);

        _viewport.Paint += (_, e) => DrawViewport(e.Graphics);
        _viewport.MouseDown += (_, e) =>
        {
            if (IsPauseMenuOpen && e.Button == MouseButtons.Left)
            {
                HandlePauseMenuClick(e.Location);
                return;
            }

            SetUiInputCapture(false, "viewport");

            if (!_mousePositionInitialized)
            {
                _lastMousePos = e.Location;
                _mousePositionInitialized = true;
            }

            if (e.Button == MouseButtons.Left)
                _leftMouseDown = true;
            if (e.Button == MouseButtons.Middle)
                _middleMouseDown = true;
            if (e.Button == MouseButtons.Right)
                _rightMouseDown = true;

            _lastMousePos = e.Location;
        };
        _viewport.MouseUp += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
                _leftMouseDown = false;
            if (e.Button == MouseButtons.Middle)
                _middleMouseDown = false;
            if (e.Button == MouseButtons.Right)
                _rightMouseDown = false;
        };
        _viewport.MouseMove += (_, e) =>
        {
            if (!_mousePositionInitialized)
            {
                _lastMousePos = e.Location;
                _mousePositionInitialized = true;
                return;
            }

            var deltaX = e.X - _lastMousePos.X;
            var deltaY = e.Y - _lastMousePos.Y;
            _lastMousePos = e.Location;

            if (IsPauseMenuOpen)
                return;

            _mouseDeltaX += deltaX;
            _mouseDeltaY += deltaY;
        };

        root.Panel1.Controls.Add(_viewport);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(12),
        };
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var logTitle = new Label
        {
            Text = "Игровой лог",
            Font = new Font("Consolas", 12, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8),
        };

        var notePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 8, 0, 0),
        };
        notePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        notePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        notePanel.Controls.Add(_noteInput, 0, 0);
        notePanel.Controls.Add(_saveNoteButton, 1, 0);

        right.Controls.Add(logTitle);
        right.Controls.Add(_logBox);
        right.Controls.Add(notePanel);

        root.Panel2.Controls.Add(right);

        _saveNoteButton.Click += (_, _) => SaveOperatorNote();
        _noteInput.KeyDown += (_, e) =>
        {
            SetUiInputCapture(true, "note-input");

            if (IsGameplayBoundKey(e.KeyCode))
            {
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                SetUiInputCapture(false, "note-input-escape");
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SaveOperatorNote();
            }
        };

        _noteInput.Enter += (_, _) => SetUiInputCapture(true, "note-input");
        _noteInput.MouseDown += (_, _) => SetUiInputCapture(true, "note-input");
        _saveNoteButton.MouseDown += (_, _) => SetUiInputCapture(true, "save-button");
        _logBox.MouseDown += (_, _) => SetUiInputCapture(true, "log-box");

        KeyDown += (_, e) =>
        {
            if (_uiInputCaptureActive && !IsPauseMenuOpen)
                return;

            if (_keysDown.Add(e.KeyCode))
                _keysPressed.Add(e.KeyCode);
        };

        KeyUp += (_, e) =>
        {
            _keysDown.Remove(e.KeyCode);
            _keysPressed.Remove(e.KeyCode);
        };
    }

    private void InitializeRuntime()
    {
        EnsureAdminFlagsInitialized();
        var buildStamp = ResolveBuildStamp();

        var contentRoot = ResolveContentRoot();
        var runtimeDir = Path.Combine(AppContext.BaseDirectory, "runtime");
        Directory.CreateDirectory(runtimeDir);
        _sessionLogPath = Path.Combine(runtimeDir, "desktop-session.log");
        _sessionCurrentLogPath = Path.Combine(runtimeDir, "desktop-session-current.log");
        _notesLogPath = Path.Combine(runtimeDir, "operator-notes.log");
        _saveGamePath = Path.Combine(runtimeDir, DesktopSaveFileName);
        _optionsPath = Path.Combine(runtimeDir, DesktopOptionsFileName);
        var options = _menuOptionsStore.LoadOrCreate(_optionsPath, BuildDefaultDesktopOptions);
        ApplyDesktopOptions(options);
        var logger = new FileLogger(Path.Combine(runtimeDir, "desktop-client.log"));

        var provider = new A0ContentProvider(contentRoot);
        var factory = new ContentDrivenSceneFactory(provider, logger);
        _bundle = factory.Build();

        _hints = new HintSystem();
        _hints.RegisterFromZones(_bundle.HubRuntime.Zones);

        _objectiveHud = new ObjectiveHUD(_bundle.ObjectiveTracker);
        _objectiveHud.Update();

        _missionSlots = Session10MissionCatalog.BuildInitialBoard(IsAdminMode);
        _archivePreviewRooms = UnlockHardArchivePreviewEnabled
            ? Session10MissionCatalog.InitialArchivePreviewRooms
            : [];

        _logBox.Clear();
        AppendUiLog("[СИСТЕМА] Клиент пролога запущен.");
        AppendUiLog($"[СИСТЕМА] Build: {buildStamp}");
        AppendUiLog($"[СИСТЕМА] Контент: {contentRoot}");
        AppendUiLog($"[СИСТЕМА] Режим доступа: {_runtimeProfile.AccessMode}");
        AppendUiLog($"[СИСТЕМА] Unlock-флаги: rooms={UnlockAllRoomsEnabled}, terminals={UnlockAllTerminalsEnabled}, archive={UnlockHardArchivePreviewEnabled}");
        AppendUiLog("[УПРАВЛЕНИЕ] Активный профиль controls задает движение/поворот/камеру/бинды. Все параметры доступны в ESC -> Options.");
        AppendUiLog("[ТЕСТ] Шаг 1: нажмите E у capsule_exit -> bio_scanner");
        AppendUiLog("[ТЕСТ] Шаг 3: нажмите E у supply_terminal -> drone_dock -> core_console");
        AppendUiLog("[ТЕСТ] Шаг 4: после OperationAccess пройдите цепочку archive_control -> archive_corridor -> entry_octagon -> index_vestibule -> research_room_01 -> stack_ring_preview");
        AppendUiLog("[ТЕСТ] Шаг 5: проверьте op_terminal + research_terminal + gallery_overlook + archive_gate");
        AppendUiLog("[ТЕСТ] Шаг 6: проверьте Q/SPACE/ESC/R и фиксируйте результат в логе");
        if (IsAdminMode)
            AppendUiLog("[ТЕСТ] Шаг 7 (ADMIN): mission_board / tech_hall / commerce_hall / archive_preview / tool_bench / archive_control");

        AppendUiLog("[SETTINGS] ESC открывает pause-меню: Play / Save / Load / Options / Exit. В Controls доступны 5 пресетов и custom-профили.");
        AppendUiLog("[ЗАМЕТКИ] Введите текст ниже и нажмите Enter/Сохранить. Заметка попадет в runtime/operator-notes.log.");
        AppendUiLog("[BOT] Я веду авто-проверку. Отвечай командами: /ok <этап> или /fail <этап> <причина>.");
        AppendUiLog("[BOT] Вопрос запуска: удалось начать движение и обзор? (ответь /ok launch или /fail launch причина)");

        _statusMessage = _runtimeProfile.AccessMode == WorldAccessMode.Admin
            ? "Режим ADMIN активен. Превью-комнаты разблокированы."
            : "Оператор, пробуждение завершено.";
        _pauseMenuOpen = false;
        _escMenuPage = EscMenuPage.Root;
        _activeOptionsCategory = OptionsCategory.Controls;
        _activeOptionsPage = 0;
        _menuHitTargets.Clear();
        _activeTerminal = null;
        _focusedObjectId = null;
        _focusedObjectName = null;
        _focusedExtraId = null;
        _focusedExtraName = null;
        _controllerState.ResetFromFacing(_bundle.Player.Facing);
        _controllerState.IsUiInputCaptured = false;
        _leftMouseDown = false;
        _middleMouseDown = false;
        _rightMouseDown = false;
        _mouseDeltaX = 0f;
        _mouseDeltaY = 0f;
        _mousePositionInitialized = false;
        _protocolObjectiveApplied = false;
        _lastImperativeKey = null;
        _lastSceneTitle = string.Empty;
        _lastHeartbeatPos = _bundle.Player.Position;
        _lastPlayerPos = _bundle.Player.Position;
        _blockedMovementMs = 0d;
        _isPathBlocked = false;
        _lastIntegrationProbeMs = 0;
        _lastCheckpointPhaseRank = PhaseRank(_bundle.HubRuntime.CurrentPhase);
        _operationQuestionIssued = false;
        _actOneFlowQuestionIssued = false;
        _actOneCompleteQuestionIssued = false;
        _uiInputCaptureActive = false;
        _hardArchivePartialUnlock = false;
        _entryOctagonUnlocked = false;
        _indexVestibuleUnlocked = false;
        _researchRoomUnlocked = false;
        _stackRingPreviewUnlocked = false;
        _lastArchiveChainCompleted = GetArchiveChainCompletedSteps();
        _lastArchiveChainNextNode = GetArchiveChainNextNodeId();

        _lastLoggedPhase = _bundle.HubRuntime.CurrentPhase;
        _lastLoggedObjectiveId = _bundle.ObjectiveTracker.GetActive()?.ObjectiveId;
        _lastLoggedFocus = null;
        _lastLoggedTerminalId = null;
        _lastLoggedCameraMode = _bundle.Session.Camera.ActiveMode;
        _lastLoggedProtocolZero = _bundle.PrologueTracker.IsProtocolZeroUnlocked;
        _lastLoggedStatusMessage = _statusMessage;
        _lastHeartbeatMs = 0;

        EnsureSessionLogInitialized();

        WriteSessionLog("SESSION", "========== NEW DESKTOP SESSION ==========");
        WriteSessionLog("SYSTEM", $"Boot time={DateTimeOffset.Now:O}");
        WriteSessionLog("SYSTEM", $"Build={buildStamp}");
        WriteSessionLog("INIT", $"SceneBundle loaded: zones={_bundle.HubRuntime.Zones.Count}; interactables={_bundle.HubRuntime.Zones.SelectMany(z => z.Objects).Count()}");
        WriteSessionLog("INIT", $"Player spawn: ({_bundle.Player.Position.X:F2},{_bundle.Player.Position.Y:F2},{_bundle.Player.Position.Z:F2})");
        WriteSessionLog("INIT", $"Camera init: mode={_bundle.Session.Camera.ActiveMode}; pos=({_bundle.Session.Camera.Position.X:F2},{_bundle.Session.Camera.Position.Y:F2},{_bundle.Session.Camera.Position.Z:F2})");
        WriteSessionLog("SYSTEM", $"Mode={_runtimeProfile.AccessMode}; unlockRooms={UnlockAllRoomsEnabled}; unlockTerminals={UnlockAllTerminalsEnabled}; archivePreview={UnlockHardArchivePreviewEnabled}");
        WriteSessionLog("SYSTEM", "Controls: bindings and mouse mappings loaded from desktop-options; ESC -> Options for full custom control tuning.");
        WriteSessionLog(
            "CONTROL",
            $"activeProfile={_activeControlProfileId}; scheme={_controlProfile.SchemeId}; cameraYaw={GetCurrentCameraYaw():F3}; strictUiLock={StrictUiInputLockEnabled}; sensitivityH={_controlProfile.OrbitSensitivity:F4}; sensitivityV={_controlProfile.OrbitSensitivityVertical:F4}; smoothing={_controlProfile.CameraSmoothing:F2}; maxOrbitYaw={_controlProfile.MaxOrbitOffsetYaw:F2}; maxOrbitVertical={_controlProfile.MaxOrbitVerticalOffset:F2}; deadzone={_controlProfile.Deadzone:F3}");
        WriteSessionLog(
            "WORLD3D",
            $"envModules={EnvironmentModules.Length}; statics={WhiteboxStatics.Count}; contourSizes={ContourRenderRegistry.SizeByModelId.Count}; fxSteam={_fxCapsuleSteamEnabled}; fxCore={_fxCoreHologramEnabled}; fxRoute={_fxArchiveRouteGuideEnabled}");
        WriteSessionLog("TEST", "1) Capsule -> 2) Biometrics -> 3) Logistics -> 4) Drone -> 5) CORE -> 6) Operations/Research/Gallery/Gate");
        WriteSessionLog("NOTES", "Operator notes file: runtime/operator-notes.log");
        WriteSessionLog("LOG", "Current session mirror: runtime/desktop-session-current.log");
        WriteSessionLog("BOT", "Smart analyzer online; expected answers: /ok <step> or /fail <step> <reason>");
        EmitImperativeDirective(force: true);
    }

    private void TickFrame()
    {
        if (_bundle is null || _hints is null || _objectiveHud is null)
            return;

        var nowTicks = _clock.ElapsedTicks;
        var dt = (float)(nowTicks - _lastTicks) / Stopwatch.Frequency;
        _lastTicks = nowTicks;
        dt = Math.Clamp(dt, 0.001f, 0.05f);

        var advanceDialogue = !_uiInputCaptureActive && ConsumePressed(_inputBindings.DialogueAdvance);
        var closeOverlay = ConsumePressed(_inputBindings.PauseMenu);
        var reset = !_uiInputCaptureActive && ConsumePressed(_inputBindings.ResetSession);

        if (advanceDialogue)
            WriteSessionLog("INPUT", "SPACE pressed (dialogue advance)");
        if (closeOverlay)
            WriteSessionLog("INPUT", "ESC pressed (menu/overlay action)");
        if (reset)
            WriteSessionLog("INPUT", "R pressed (session reset)");

        if (reset)
        {
            InitializeRuntime();
            return;
        }

        if (closeOverlay)
        {
            if (_activeTerminal is not null)
            {
                _activeTerminal = null;
                AppendUiLog("[РЕЗУЛЬТАТ] Терминал закрыт.");
                WriteSessionLog("RESULT", "Overlay closed by ESC");
            }
            else
            {
                TogglePauseMenu();
            }
        }

        _controllerState.IsUiInputCaptured = _uiInputCaptureActive || IsPauseMenuOpen;

        if (_bundle.DialoguePlayer.IsPlaying && advanceDialogue)
        {
            _bundle.DialoguePlayer.Advance();
            AppendUiLog("[ДИАЛОГ] Переход к следующей реплике.");
            WriteSessionLog("DIALOGUE", "Advanced to next line");
            if (_bundle.DialoguePlayer.IsComplete)
            {
                _bundle.DialoguePlayer.Stop();
                AppendUiLog("[РЕЗУЛЬТАТ] Диалог завершен.");
                WriteSessionLog("DIALOGUE", "Dialogue completed and stopped");
            }
        }

        var blockedByOverlay = _activeTerminal is not null || _uiInputCaptureActive || IsPauseMenuOpen;
        var interactPressed = !blockedByOverlay && ConsumePressed(_inputBindings.Interact);
        var cameraTogglePressed = !blockedByOverlay && ConsumePressed(_inputBindings.CameraToggle);

        var moveForwardHeld = IsBoundHeld(_inputBindings.MoveForward);
        var moveBackwardHeld = IsBoundHeld(_inputBindings.MoveBackward);
        var moveLeftHeld = IsBoundHeld(_inputBindings.MoveLeft);
        var moveRightHeld = IsBoundHeld(_inputBindings.MoveRight);
        var turnLeftHeld = IsBoundHeld(_inputBindings.TurnLeft);
        var turnRightHeld = IsBoundHeld(_inputBindings.TurnRight);

        var keyboardOrbitModifierActive =
            _controlProfile.EnableCameraOrbitInput &&
            _inputBindings.OrbitModifier != Keys.None &&
            IsBoundHeld(_inputBindings.OrbitModifier);

        ConsumeMouseDelta(out var mouseDeltaX, out var mouseDeltaY);

        var mouseOrbitActive =
            !blockedByOverlay &&
            _controlProfile.EnableCameraOrbitInput &&
            IsMouseButtonActive(_controlProfile.CameraOrbitButton);

        if (mouseOrbitActive)
            ControlV2Pipeline.ApplyOrbitDelta(_controllerState, mouseDeltaX, mouseDeltaY, _controlProfile);

        _controllerState.IsOrbitModifierActive = mouseOrbitActive || keyboardOrbitModifierActive;

        var mouseTurnInput = 0f;
        if (!blockedByOverlay && _controlProfile.EnableMousePlayerTurn && IsMouseButtonActive(_controlProfile.MousePlayerTurnButton))
        {
            var turnSign = _controlProfile.InvertMousePlayerTurn ? -1f : 1f;
            mouseTurnInput = mouseDeltaX * _controlProfile.MousePlayerTurnSensitivity * turnSign;
        }

        var mouseForwardInput = 0f;
        var mouseLateralInput = 0f;
        if (!blockedByOverlay && _controlProfile.EnableMousePlayerMove && IsMouseButtonActive(_controlProfile.MousePlayerMoveButton))
        {
            var forwardSign = _controlProfile.InvertMousePlayerMoveForward ? -1f : 1f;
            var lateralSign = _controlProfile.InvertMousePlayerMoveLateral ? -1f : 1f;
            mouseForwardInput = -mouseDeltaY * _controlProfile.MousePlayerMoveForwardSensitivity * forwardSign;
            mouseLateralInput = mouseDeltaX * _controlProfile.MousePlayerMoveLateralSensitivity * lateralSign;
        }

        var moveDirection = _bundle is null
            ? Vec3.Zero
            : ControlV2Pipeline.ResolveMoveDirection(
                _controllerState,
                _controlProfile,
                _bundle.Player.Facing,
                GetCurrentCameraYaw(),
                moveForwardHeld,
                moveBackwardHeld,
                moveLeftHeld,
                moveRightHeld,
                blockedByOverlay,
                dt,
                additionalForwardInput: mouseForwardInput,
                additionalLateralInput: mouseLateralInput);

        var movementIntent = moveDirection.LengthSquared() > 0.0001f;
        var normalizedMoveDirection = movementIntent ? moveDirection.Normalized() : Vec3.Zero;

        if (_bundle is not null)
        {
            _bundle.Player.Facing = ControlV2Pipeline.UpdateFacingFromInput(
                _controllerState,
                _controlProfile,
                _bundle.Player.Facing,
                moveDirection,
                turnLeftHeld,
                turnRightHeld,
                GetCurrentCameraYaw(),
                dt,
                blockedByOverlay,
                additionalTurnInput: mouseTurnInput);

            var playerTurnIntent = MathF.Abs(mouseTurnInput) > MathF.Max(_controlProfile.Deadzone, 0.0001f)
                || turnLeftHeld
                || turnRightHeld;

            ControlV2Pipeline.UpdateFollowCameraYaw(
                _controllerState,
                _bundle.Player.Facing,
                movementIntent,
                dt,
                _controlProfile,
                playerTurnIntent);
        }

        var input = new InputSnapshot
        {
            MoveDirection = normalizedMoveDirection,
            InteractPressed = interactPressed,
            CameraTogglePressed = cameraTogglePressed,
            ApplyFacingDirection = true,
            FacingDirection = _bundle.Player.Facing,
        };

        if (input.CameraTogglePressed)
            WriteSessionLog("INPUT", "Q pressed (camera toggle)");
        if (interactPressed)
            WriteSessionLog("INPUT", "E pressed (interaction attempt)");

        if (mouseOrbitActive)
            _statusMessage = "Режим камеры: активен орбитальный ввод мышью.";

        var beforeFramePos = _bundle.Player.Position;
        var frame = _bundle.Session.ProcessFrame(input, dt);

        var target = _bundle.Session.PlayerCtrl.TryGetInteractionTarget(_bundle.HubRuntime.CurrentPhase);
        _focusedObjectId = target?.ObjectId;
        _focusedObjectName = target?.DisplayName;

        var extraTarget = FindExtraInteractionTarget(_bundle.Player.Position);
        _focusedExtraId = extraTarget?.Id;
        _focusedExtraName = extraTarget?.DisplayName;

        if (target is not null)
        {
            _focusedExtraId = null;
            _focusedExtraName = null;
        }

        _hints.Update(target?.ObjectId);

        if (frame.Interaction is not null)
        {
            if (frame.Interaction.Success && frame.Interaction.ObjectId is { } usedId)
            {
                _bundle.PrologueTracker.RecordVisit(usedId);
            }

            if (!string.IsNullOrWhiteSpace(frame.Interaction.Message))
                _statusMessage = frame.Interaction.Message;

            if (frame.Interaction.TerminalScreen is not null)
                _activeTerminal = frame.Interaction.TerminalScreen;

            AppendUiLog($"[INTERACT] {frame.Interaction.ObjectId} -> {(frame.Interaction.Success ? "OK" : "FAIL")} | {frame.Interaction.Message}");
            WriteSessionLog(
                "INTERACT",
                $"object={frame.Interaction.ObjectId}; success={frame.Interaction.Success}; phase={frame.Phase}; message={frame.Interaction.Message}");

            if (frame.Interaction.GrantedItems is { Count: > 0 })
            {
                AppendUiLog("[LOOT] " + string.Join(", ", frame.Interaction.GrantedItems.Select(i => i.Name)));
                WriteSessionLog("LOOT", string.Join(", ", frame.Interaction.GrantedItems.Select(i => i.ItemId)));
            }
        }

        if (interactPressed && frame.Interaction is null && _focusedExtraId is not null)
            HandleExtraInteraction(_focusedExtraId);
        else if (interactPressed && frame.Interaction is null && TryCanonicalInteractionFallback())
        {
            // Fallback processed and logged inside TryCanonicalInteractionFallback.
        }
        else if (interactPressed && frame.Interaction is null && _focusedExtraId is null)
        {
            AppendUiLog("[РЕЗУЛЬТАТ] Взаимодействие не выполнено: цель не найдена.");
            WriteSessionLog("INTERACT", "result=no-target");
        }

        if (_bundle.PrologueTracker.IsProtocolZeroUnlocked && !_protocolObjectiveApplied)
        {
            var activeObjective = _bundle.ObjectiveTracker.GetActive();
            if (activeObjective is not null)
                _bundle.ObjectiveTracker.Complete(activeObjective.ObjectiveId);

            _bundle.ObjectiveTracker.SetActive("UI_OBJ_PROLOGUE_DONE");
            _statusMessage = "Протокол Ноль разблокирован. Пролог завершен.";
            AppendUiLog("[СИСТЕМА] Protocol Zero разблокирован -> цель пролога завершена.");
            WriteSessionLog("PROGRESSION", "Protocol Zero unlocked; objective switched to UI_OBJ_PROLOGUE_DONE");
            _protocolObjectiveApplied = true;
        }

        AnalyzePathingAndBackgroundIssues(beforeFramePos, dt, blockedByOverlay);

        RecordBackgroundTelemetry();
        RunSmartTestBot();
        EmitImperativeDirective(force: false);

        _objectiveHud.Update();
        UpdateFps();
        _viewport.Invalidate();
    }

    private void UpdateFps()
    {
        _framesInWindow++;
        var elapsed = _clock.ElapsedMilliseconds - _fpsWindowStart;
        if (elapsed >= 500)
        {
            _fps = _framesInWindow * 1000f / elapsed;
            _framesInWindow = 0;
            _fpsWindowStart = _clock.ElapsedMilliseconds;
        }
    }

    private void DrawViewport(Graphics g)
    {
        if (_bundle is null || _hints is null || _objectiveHud is null)
            return;

        var drawStart = Stopwatch.GetTimestamp();

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.FromArgb(12, 15, 24));

        DrawCinematicBackground(g);
        DrawFloorGrid(g);
        DrawEnvironmentShell(g);
        DrawStaticWhitebox(g);
        DrawZonesAndObjects(g);
        DrawExtraNodes(g);
        DrawPhaseEffects(g);
        DrawPlayer(g);
        DrawHud(g);
        DrawDialogueOverlay(g);
        DrawTerminalOverlay(g);
        DrawPauseMenuOverlay(g);
        DrawCinematicVignette(g);

        _lastVisibleInteractables = _bundle.HubRuntime.Zones.SelectMany(z => z.Objects).Count();
        _lastVisibleExtraNodes = ExtraNodes.Count(node => IsExtraNodeAvailable(node.Id));

        var drawEnd = Stopwatch.GetTimestamp();
        var elapsedMs = (drawEnd - drawStart) * 1000d / Stopwatch.Frequency;
        _lastDrawMs = elapsedMs;
        _drawMsAccumulator += elapsedMs;
        _drawMsSamples++;
        _drawMsAverage = _drawMsAccumulator / _drawMsSamples;
    }

    private void DrawCinematicBackground(Graphics g)
    {
        if (_bundle is null)
            return;

        var tone = ResolveSceneTone(_bundle.HubRuntime.CurrentPhase);
        using var bg = new LinearGradientBrush(
            new Rectangle(0, 0, _viewport.Width, _viewport.Height),
            tone.Top,
            tone.Bottom,
            LinearGradientMode.Vertical);
        g.FillRectangle(bg, 0, 0, _viewport.Width, _viewport.Height);

        var t = (float)_clock.Elapsed.TotalSeconds;
        var glowX = _viewport.Width * (0.5f + 0.24f * MathF.Sin(t * 0.22f));
        var glowY = _viewport.Height * (0.34f + 0.11f * MathF.Cos(t * 0.31f));
        using var glow = new SolidBrush(Color.FromArgb(54, tone.Glow));
        g.FillEllipse(glow, glowX - 280, glowY - 180, 560, 360);

        using var fog = new SolidBrush(Color.FromArgb(42, tone.Fog));
        g.FillRectangle(fog, 0, _viewport.Height - 220, _viewport.Width, 220);

        using var scanPen = new Pen(Color.FromArgb(18, 190, 215, 245), 1f);
        for (var y = 0; y < _viewport.Height; y += 8)
            g.DrawLine(scanPen, 0, y, _viewport.Width, y);
    }

    private void DrawCinematicVignette(Graphics g)
    {
        using var edge = new SolidBrush(Color.FromArgb(84, 3, 6, 12));
        g.FillRectangle(edge, 0, 0, _viewport.Width, 18);
        g.FillRectangle(edge, 0, _viewport.Height - 18, _viewport.Width, 18);
        g.FillRectangle(edge, 0, 0, 20, _viewport.Height);
        g.FillRectangle(edge, _viewport.Width - 20, 0, 20, _viewport.Height);
    }

    private void DrawEnvironmentShell(Graphics g)
    {
        var pulse = 0.5f + 0.5f * MathF.Sin((float)_clock.Elapsed.TotalSeconds * 1.8f);
        foreach (var module in EnvironmentModules)
        {
            var color = module.Emissive
                ? Color.FromArgb(
                    180,
                    Math.Min(255, (int)(module.Color.R * (0.85f + 0.35f * pulse))),
                    Math.Min(255, (int)(module.Color.G * (0.85f + 0.35f * pulse))),
                    Math.Min(255, (int)(module.Color.B * (0.85f + 0.35f * pulse))))
                : module.Color;

            DrawMarkerCube(g, module.Position, module.Size, color, module.Emissive ? 2.8f : 1.8f);
        }
    }

    private void DrawFloorGrid(Graphics g)
    {
        using var gridPen = new Pen(Color.FromArgb(35, 70, 90));

        for (var z = -16; z <= 8; z += 2)
            DrawWorldLine(g, new Vec3(-14, 0, z), new Vec3(14, 0, z), gridPen);

        for (var x = -14; x <= 14; x += 2)
            DrawWorldLine(g, new Vec3(x, 0, -16), new Vec3(x, 0, 8), gridPen);
    }

    private void DrawStaticWhitebox(Graphics g)
    {
        foreach (var item in WhiteboxStatics.Values)
            DrawMarkerCube(g, item.Pos, item.Size, item.Color, 2f);

        if (UnlockHardArchivePreviewEnabled)
        {
            DrawMarkerCube(g, new Vec3(0.0f, 2.2f, -15.0f), new Vec3(4.5f, 4.5f, 1.2f), Color.FromArgb(90, 210, 120), 3f);

            var previewPos = new[]
            {
                new Vec3(-3.8f, 0.8f, -18.5f),
                new Vec3(0.0f, 0.8f, -19.4f),
                new Vec3(3.8f, 0.8f, -18.5f),
                new Vec3(-2.2f, 1.8f, -21.6f),
                new Vec3(2.2f, 1.8f, -21.6f),
            };

            foreach (var pos in previewPos)
                DrawMarkerCube(g, pos, new Vec3(1.0f, 1.6f, 1.0f), Color.FromArgb(95, 150, 230), 2.5f);
        }

        if (PurchaseSimulationEnabled)
        {
            DrawMarkerCube(g, new Vec3(-9.5f, 1.2f, -7.8f), new Vec3(1.2f, 2.0f, 1.2f), Color.FromArgb(230, 210, 110), 2.5f);
            DrawMarkerCube(g, new Vec3(9.5f, 1.2f, -7.8f), new Vec3(1.2f, 2.0f, 1.2f), Color.FromArgb(110, 210, 230), 2.5f);
        }
    }

    private void DrawPhaseEffects(Graphics g)
    {
        if (_bundle is null)
            return;

        if (_fxCapsuleSteamEnabled && _bundle.HubRuntime.CurrentPhase <= HubRhythmPhase.Identification)
            DrawCapsuleSteamFx(g);

        if (_fxCoreHologramEnabled && _bundle.HubRuntime.CurrentPhase >= HubRhythmPhase.Activation)
            DrawCoreHologramFx(g);

        if (_fxArchiveRouteGuideEnabled && _bundle.HubRuntime.CurrentPhase >= HubRhythmPhase.OperationAccess)
            DrawArchiveRouteGuideFx(g);
    }

    private void DrawCapsuleSteamFx(Graphics g)
    {
        var t = (float)_clock.Elapsed.TotalSeconds;
        for (var i = 0; i < 11; i++)
        {
            var phase = t * 0.9f + i * 0.47f;
            var x = -8.0f + MathF.Sin(phase * 1.3f) * 0.7f;
            var y = 0.8f + ((phase % 1.0f) * 1.7f);
            var z = -6.0f + MathF.Cos(phase * 1.7f) * 0.5f;
            var alpha = 38 + (int)(24f * (0.5f + 0.5f * MathF.Sin(phase * 2.1f)));
            var color = Color.FromArgb(Math.Clamp(alpha, 24, 96), 186, 214, 232);
            DrawMarkerCube(g, new Vec3(x, y, z), new Vec3(0.16f, 0.16f, 0.16f), color, 1.2f);
        }
    }

    private void DrawCoreHologramFx(Graphics g)
    {
        var t = (float)_clock.Elapsed.TotalSeconds;
        var pulse = 0.5f + 0.5f * MathF.Sin(t * 2.6f);
        var ringY = 2.05f + 0.12f * MathF.Sin(t * 1.4f);

        for (var i = 0; i < 14; i++)
        {
            var angle = t * 1.7f + (MathF.PI * 2f * i / 14f);
            var radius = 1.55f + 0.12f * MathF.Sin(t * 1.9f + i * 0.4f);
            var x = radius * MathF.Cos(angle);
            var z = radius * MathF.Sin(angle);
            var color = Color.FromArgb(120, 90, 190 + (int)(40f * pulse), 255);
            DrawMarkerCube(g, new Vec3(x, ringY, z), new Vec3(0.13f, 0.13f, 0.13f), color, 1.5f);
        }

        DrawMarkerCube(g, new Vec3(0f, 2.2f, 0f), new Vec3(0.35f, 0.6f, 0.35f), Color.FromArgb(125, 110, 220, 255), 2f);
    }

    private void DrawArchiveRouteGuideFx(Graphics g)
    {
        var pathPoints = new List<Vec3>();
        foreach (var nodeId in ArchiveEntryChain.OrderedNodes)
        {
            if (!IsArchiveChainNodeUnlocked(nodeId))
                break;

            if (ResolveExtraNodePosition(nodeId, out var position))
                pathPoints.Add(position + new Vec3(0f, 1.7f, 0f));
        }

        if (pathPoints.Count > 1)
        {
            using var pathPen = new Pen(Color.FromArgb(156, 246, 188, 88), 2.2f);
            for (var i = 0; i < pathPoints.Count - 1; i++)
                DrawWorldLine(g, pathPoints[i], pathPoints[i + 1], pathPen);
        }

        var nextNodeId = GetArchiveChainNextNodeId();
        if (!string.IsNullOrWhiteSpace(nextNodeId) && ResolveExtraNodePosition(nextNodeId, out var nextPos))
        {
            var pulse = 0.5f + 0.5f * MathF.Sin((float)_clock.Elapsed.TotalSeconds * 3.2f);
            var color = Color.FromArgb(180, 246, 160 + (int)(70f * pulse), 90);
            DrawMarkerCube(g, nextPos + new Vec3(0f, 1.65f, 0f), new Vec3(0.42f, 0.42f, 0.42f), color, 2.8f);
        }
    }

    private static bool ResolveExtraNodePosition(string nodeId, out Vec3 position)
    {
        var node = ExtraNodes.FirstOrDefault(n => n.Id.Equals(nodeId, StringComparison.Ordinal));
        if (node is null)
        {
            position = Vec3.Zero;
            return false;
        }

        position = node.Position;
        return true;
    }

    private bool IsArchiveChainNodeUnlocked(string nodeId)
    {
        return nodeId switch
        {
            ArchiveEntryChain.ArchiveControl => true,
            ArchiveEntryChain.ArchiveCorridor => _hardArchivePartialUnlock,
            ArchiveEntryChain.EntryOctagon => _entryOctagonUnlocked,
            ArchiveEntryChain.IndexVestibule => _indexVestibuleUnlocked,
            ArchiveEntryChain.ResearchRoom01 => _researchRoomUnlocked,
            ArchiveEntryChain.StackRingPreview => _stackRingPreviewUnlocked,
            _ => false,
        };
    }

    private int GetArchiveChainCompletedSteps()
    {
        return ArchiveEntryChain.GetCompletedSteps(
            _hardArchivePartialUnlock,
            _entryOctagonUnlocked,
            _indexVestibuleUnlocked,
            _researchRoomUnlocked,
            _stackRingPreviewUnlocked);
    }

    private string? GetArchiveChainNextNodeId()
    {
        return ArchiveEntryChain.GetNextNodeId(
            _hardArchivePartialUnlock,
            _entryOctagonUnlocked,
            _indexVestibuleUnlocked,
            _researchRoomUnlocked,
            _stackRingPreviewUnlocked);
    }

    private static string ResolveArchiveChainNodeLabel(string? nodeId)
    {
        return nodeId switch
        {
            ArchiveEntryChain.ArchiveControl => "Archive Control",
            ArchiveEntryChain.ArchiveCorridor => "Archive Corridor",
            ArchiveEntryChain.EntryOctagon => "Entry Octagon",
            ArchiveEntryChain.IndexVestibule => "Index Vestibule",
            ArchiveEntryChain.ResearchRoom01 => "Research Room 01",
            ArchiveEntryChain.StackRingPreview => "Stack Ring Preview",
            _ => "Done",
        };
    }

    private void DrawZonesAndObjects(Graphics g)
    {
        if (_bundle is null)
            return;

        foreach (var obj in _bundle.HubRuntime.Zones.SelectMany(z => z.Objects))
        {
            var isFocused = obj.Id == _focusedObjectId;
            var isActive = obj.IsActiveIn(_bundle.HubRuntime.CurrentPhase);
            var visited = _bundle.PrologueTracker.HasVisited(obj.Id);
            var t = (float)_clock.Elapsed.TotalSeconds;

            var contourDescriptor = ContourRenderRegistry.ResolveDescriptor(
                obj.ModelId,
                ContourRenderRegistry.GetInteractableFallbackSize(obj.InteractiveType));
            var baseSize = contourDescriptor.Size;

            var offsetY = obj.InteractiveType == InteractiveType.Npc
                ? 1.2f + 0.12f * MathF.Sin(t * 2.2f)
                : 1.2f;

            var renderState = ContourRenderRegistry.ResolveRenderState(isActive, isFocused, visited);
            var color = ResolveContourColor(renderState);

            var center = new Vec3(obj.Position.X, obj.Position.Y + offsetY, obj.Position.Z);
            DrawMarkerCube(g, center, baseSize, color, isFocused ? 3.5f : 2f);

            var p = Project(center + new Vec3(0, 1.0f, 0));
            if (p is not null)
            {
                using var brush = new SolidBrush(Color.FromArgb(225, 235, 245));
                g.DrawString(obj.DisplayName, Font, brush, p.Value.X - 40, p.Value.Y - 8);
            }
        }
    }

    private void DrawExtraNodes(Graphics g)
    {
        if (_bundle is null)
            return;

        foreach (var node in ExtraNodes)
        {
            if (!IsExtraNodeAvailable(node.Id))
                continue;

            var focused = node.Id == _focusedExtraId;
            var color = focused
                ? Color.FromArgb(255, 225, 95)
                : node.Id switch
                {
                    "commerce_hall" or "commerce_desk" => Color.FromArgb(216, 190, 92),
                    "tech_hall" or "tool_bench" => Color.FromArgb(110, 190, 230),
                    "archive_preview" or "archive_control" => Color.FromArgb(140, 110, 220),
                    "mission_board" => Color.FromArgb(244, 206, 84),
                    "research_lab" => Color.FromArgb(96, 186, 238),
                    _ => Color.FromArgb(188, 188, 196),
                };

            var contourDescriptor = ContourRenderRegistry.ResolveDescriptor(
                node.ModelId,
                ContourRenderRegistry.GetExtraNodeFallbackSize(node.Id, node.ModelId));
            var size = contourDescriptor.Size;

            DrawMarkerCube(g, new Vec3(node.Position.X, node.Position.Y + 1.1f, node.Position.Z), size, color, focused ? 3.2f : 2.2f);

            var p = Project(new Vec3(node.Position.X, node.Position.Y + 2.1f, node.Position.Z));
            if (p is not null)
            {
                using var brush = new SolidBrush(Color.FromArgb(220, 236, 246));
                g.DrawString(node.DisplayName, Font, brush, p.Value.X - 52, p.Value.Y - 8);
            }
        }
    }

    private void DrawPlayer(Graphics g)
    {
        if (_bundle is null)
            return;

        var p = _bundle.Player.Position;
        DrawMarkerCube(g, new Vec3(p.X, p.Y + 0.9f, p.Z), new Vec3(0.9f, 1.8f, 0.9f), Color.FromArgb(245, 245, 250), 3f);

        var facing = p + (_bundle.Player.Facing.LengthSquared() > 0.001f ? _bundle.Player.Facing.Normalized() * 1.4f : new Vec3(0, 0, -1));
        using var facingPen = new Pen(Color.FromArgb(255, 170, 80), 2f);
        DrawWorldLine(g, new Vec3(p.X, p.Y + 0.95f, p.Z), new Vec3(facing.X, p.Y + 0.95f, facing.Z), facingPen);
    }

    private void DrawHud(Graphics g)
    {
        if (_bundle is null || _hints is null || _objectiveHud is null)
            return;

        using var hudBg = new SolidBrush(Color.FromArgb(150, 8, 12, 20));
        using var textBrush = new SolidBrush(Color.FromArgb(225, 235, 245));
        using var accentBrush = new SolidBrush(Color.FromArgb(125, 210, 255));
        using var sceneBrush = new SolidBrush(Color.FromArgb(235, 240, 210, 120));
        var chainCompleted = GetArchiveChainCompletedSteps();
        var chainNext = GetArchiveChainNextNodeId();

        g.FillRectangle(hudBg, 14, 14, 600, 218);
        g.DrawString($"Scene: {ResolveSceneTone(_bundle.HubRuntime.CurrentPhase).Title}", Font, sceneBrush, 24, 24);
        g.DrawString($"Phase: {_bundle.HubRuntime.CurrentPhase}", Font, textBrush, 24, 46);
        g.DrawString($"Zone: {_bundle.Session.PlayerCtrl.GetCurrentZone()?.Id}", Font, textBrush, 24, 68);
        g.DrawString($"Objective: {_objectiveHud.DisplayText}", Font, accentBrush, 24, 90);
        g.DrawString($"Protocol Zero: {_bundle.PrologueTracker.IsProtocolZeroUnlocked}", Font, textBrush, 24, 112);
        g.DrawString($"Camera: {_bundle.Session.Camera.ActiveMode} | FPS: {_fps:F0}", Font, textBrush, 24, 134);
        if (_showExtendedHudTelemetry)
        {
            g.DrawString($"Render: {_lastDrawMs:F2} ms (avg {_drawMsAverage:F2})", Font, textBrush, 24, 156);
            g.DrawString($"Archive Chain: {chainCompleted}/{ArchiveEntryChain.OrderedNodes.Count}", Font, textBrush, 24, 178);
            g.DrawString($"Next Chain Node: {ResolveArchiveChainNodeLabel(chainNext)}", Font, accentBrush, 24, 200);
            g.DrawString($"Mode: {_runtimeProfile.AccessMode}", Font, accentBrush, 350, 134);
            g.DrawString($"FX Steam/Core/Route: {(_fxCapsuleSteamEnabled ? "ON" : "OFF")}/{(_fxCoreHologramEnabled ? "ON" : "OFF")}/{(_fxArchiveRouteGuideEnabled ? "ON" : "OFF")}", Font, accentBrush, 350, 156);
        }
        else
        {
            g.DrawString("HUD mode: compact", Font, accentBrush, 24, 156);
            g.DrawString($"Archive Chain: {chainCompleted}/{ArchiveEntryChain.OrderedNodes.Count}", Font, textBrush, 24, 178);
        }

        var inv = _bundle.Inventory.Items;
        g.FillRectangle(hudBg, _viewport.Width - 380, 14, 366, 156);
        g.DrawString("Operator: Алан Арквейн", Font, textBrush, _viewport.Width - 340, 24);
        g.DrawString($"XP: 145 | Level: 1", Font, textBrush, _viewport.Width - 340, 46);
        g.DrawString($"Inventory: {inv.Count}", Font, textBrush, _viewport.Width - 340, 68);
        g.DrawString(string.Join(", ", inv.Select(i => i.ItemId)), Font, accentBrush, _viewport.Width - 340, 90);
        g.DrawString($"Mission slots: {_missionSlots.Count}", Font, textBrush, _viewport.Width - 340, 112);
        g.DrawString($"Archive preview rooms: {_archivePreviewRooms.Count}", Font, textBrush, _viewport.Width - 340, 134);

        var prompt = BuildPrompt();
        if (!string.IsNullOrWhiteSpace(prompt))
        {
            var size = g.MeasureString(prompt, Font);
            var x = (_viewport.Width - size.Width) / 2f;
            var y = _viewport.Height - 70;
            g.FillRectangle(hudBg, x - 10, y - 6, size.Width + 20, size.Height + 12);
            g.DrawString(prompt, Font, accentBrush, x, y);
        }

        if (!string.IsNullOrWhiteSpace(_statusMessage))
            g.DrawString(_statusMessage, Font, textBrush, 24, _viewport.Height - 30);
    }

    private void DrawDialogueOverlay(Graphics g)
    {
        if (_bundle?.DialoguePlayer.CurrentLine is not { } line)
            return;

        using var bg = new SolidBrush(Color.FromArgb(210, 0, 0, 0));
        using var speaker = new SolidBrush(Color.FromArgb(140, 230, 255));
        using var text = new SolidBrush(Color.FromArgb(240, 240, 240));

        var panel = new Rectangle(40, _viewport.Height - 200, _viewport.Width - 80, 140);
        g.FillRectangle(bg, panel);
        g.DrawString(line.Speaker, Font, speaker, panel.X + 16, panel.Y + 14);
        g.DrawString(line.Text, Font, text, panel.X + 16, panel.Y + 44);
        g.DrawString("[SPACE] Continue", Font, text, panel.X + 16, panel.Bottom - 28);
    }

    private void DrawTerminalOverlay(Graphics g)
    {
        if (_activeTerminal is null)
            return;

        using var bg = new SolidBrush(Color.FromArgb(235, 9, 17, 28));
        using var border = new Pen(Color.FromArgb(220, 90, 180, 255), 2);
        using var head = new SolidBrush(Color.FromArgb(245, 215, 90));
        using var body = new SolidBrush(Color.FromArgb(235, 235, 235));

        var panel = new Rectangle(140, 90, _viewport.Width - 280, _viewport.Height - 180);
        g.FillRectangle(bg, panel);
        g.DrawRectangle(border, panel);
        g.DrawString(_activeTerminal.Title, new Font(Font, FontStyle.Bold), head, panel.X + 18, panel.Y + 16);

        float y = panel.Y + 54;
        foreach (var line in _activeTerminal.Lines)
        {
            g.DrawString(line, Font, body, panel.X + 18, y);
            y += 24;
            if (y > panel.Bottom - 42)
                break;
        }

        g.DrawString("[ESC] Close terminal", Font, body, panel.X + 18, panel.Bottom - 30);
    }

    private void DrawMarkerCube(Graphics g, Vec3 center, Vec3 size, Color color, float penWidth)
    {
        var hx = size.X * 0.5f;
        var hy = size.Y * 0.5f;
        var hz = size.Z * 0.5f;

        var corners = new[]
        {
            new Vec3(center.X - hx, center.Y - hy, center.Z - hz),
            new Vec3(center.X + hx, center.Y - hy, center.Z - hz),
            new Vec3(center.X + hx, center.Y - hy, center.Z + hz),
            new Vec3(center.X - hx, center.Y - hy, center.Z + hz),
            new Vec3(center.X - hx, center.Y + hy, center.Z - hz),
            new Vec3(center.X + hx, center.Y + hy, center.Z - hz),
            new Vec3(center.X + hx, center.Y + hy, center.Z + hz),
            new Vec3(center.X - hx, center.Y + hy, center.Z + hz),
        };

        var p = corners.Select(Project).ToArray();
        if (p.Any(pt => pt is null)) return;

        using var pen = new Pen(color, penWidth);
        DrawEdge(g, pen, p, 0, 1); DrawEdge(g, pen, p, 1, 2); DrawEdge(g, pen, p, 2, 3); DrawEdge(g, pen, p, 3, 0);
        DrawEdge(g, pen, p, 4, 5); DrawEdge(g, pen, p, 5, 6); DrawEdge(g, pen, p, 6, 7); DrawEdge(g, pen, p, 7, 4);
        DrawEdge(g, pen, p, 0, 4); DrawEdge(g, pen, p, 1, 5); DrawEdge(g, pen, p, 2, 6); DrawEdge(g, pen, p, 3, 7);
    }

    private void DrawWorldLine(Graphics g, Vec3 from, Vec3 to, Pen pen)
    {
        var p1 = Project(from);
        var p2 = Project(to);
        if (p1 is null || p2 is null) return;
        g.DrawLine(pen, p1.Value, p2.Value);
    }

    private PointF? Project(Vec3 world)
    {
        if (_bundle is null)
            return null;

        var look = _bundle.Player.Position;
        var horizontalRadius = _controlProfile.CameraDistance;
        var camYaw = GetCurrentCameraYaw();

        // Stable behind-the-back anchor with profile-driven distance/height and optional orbit offset.
        var localBackOffset = new Vec3(0f, _controlProfile.CameraHeight + _controllerState.OrbitVerticalOffset, -horizontalRadius);
        var cam = look + RotateY(localBackOffset, camYaw);

        var fwd = (look - cam).Normalized();

        var up = new Vec3(0, 1, 0);
        var right = Cross(up, fwd).Normalized();
        var trueUp = Cross(fwd, right).Normalized();

        var rel = world - cam;
        var vx = Dot(rel, right);
        var vy = Dot(rel, trueUp);
        var vz = Dot(rel, fwd);

        if (vz <= 0.15f)
            return null;

        var focal = 820f;
        var sx = _viewport.Width * 0.5f + (vx / vz) * focal;
        var sy = _viewport.Height * 0.56f - (vy / vz) * focal;

        return new PointF(sx, sy);
    }

    private string? BuildPrompt()
    {
        if (_focusedObjectId is not null && _focusedObjectName is not null)
        {
            if (_focusedObjectId == "archive_gate" && UnlockHardArchivePreviewEnabled)
                return "E - Вход в Хард-Архив (ADMIN PREVIEW)";

            return $"E - {_focusedObjectName}";
        }

        if (_focusedExtraId is not null && _focusedExtraName is not null)
            return $"E - {_focusedExtraName}";

        return null;
    }

    private ExtraInteractionNode? FindExtraInteractionTarget(Vec3 playerPos)
    {
        foreach (var node in ExtraNodes)
        {
            if (!IsExtraNodeAvailable(node.Id))
                continue;

            var d = (node.Position - playerPos).Length();
            if (d <= node.Radius)
                return node;
        }

        return null;
    }

    private bool IsExtraNodeAvailable(string nodeId)
    {
        if (_bundle is null)
            return false;

        var isOperationAccess = _bundle.HubRuntime.CurrentPhase >= HubRhythmPhase.OperationAccess;

        return nodeId switch
        {
            "commerce_hall" => isOperationAccess || UnlockAllRoomsEnabled,
            "commerce_desk" => isOperationAccess || PurchaseSimulationEnabled || UnlockAllRoomsEnabled,
            "tech_hall" => isOperationAccess || UnlockAllRoomsEnabled,
            "tool_bench" => isOperationAccess || UnlockAllRoomsEnabled,
            "archive_preview" => UnlockHardArchivePreviewEnabled,
            "mission_board" => isOperationAccess || UnlockAllTerminalsEnabled,
            "research_lab" => isOperationAccess || UnlockAllTerminalsEnabled,
            "archive_control" => isOperationAccess || UnlockHardArchivePreviewEnabled,
            "archive_corridor" => _hardArchivePartialUnlock,
            "entry_octagon" => _entryOctagonUnlocked,
            "index_vestibule" => _indexVestibuleUnlocked,
            "research_room_01" => _researchRoomUnlocked,
            "stack_ring_preview" => _stackRingPreviewUnlocked || UnlockHardArchivePreviewEnabled,
            _ => false,
        };
    }

    private void HandleExtraInteraction(string nodeId)
    {
        WriteSessionLog("INTERACT", $"extraNode={nodeId}");
        switch (nodeId)
        {
            case "commerce_hall":
                IReadOnlyList<string> commerceLines = PurchaseSimulationEnabled
                    ?
                    [
                        "Симуляция покупок активна (ADMIN).",
                        "Scanner+ // Utility",
                        "Field Pack // Support",
                        "Slot Expander // Convenience",
                    ]
                    :
                    [
                        "Зал доступен в ознакомительном режиме.",
                        "Некоторые узлы откроются после первых операций.",
                    ];

                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "commerce_hall",
                    Title = "КОММЕРЧЕСКИЙ ЗАЛ",
                    Lines = commerceLines,
                };
                _statusMessage = "Коммерческий зал подключен.";
                AppendUiLog("[HUB] Коммерческий зал: узел открыт.");
                WriteSessionLog("HUB", "Commerce hall terminal opened");
                break;

            case "tech_hall":
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "tech_hall",
                    Title = "ТЕХНИЧЕСКИЙ ЗАЛ",
                    Lines =
                    [
                        "Фрагменты -> инструменты.",
                        "Tool bench: Archive Address Inspector (preview).",
                    ],
                };
                _statusMessage = "Технический зал подключен.";
                AppendUiLog("[HUB] Технический зал: узел открыт.");
                WriteSessionLog("HUB", "Tech hall terminal opened");
                break;

            case "archive_preview":
                IReadOnlyList<string> archiveLines = _archivePreviewRooms.Count > 0
                    ? _archivePreviewRooms.Select((r, i) => $"{i + 1:00}. {r}").ToList()
                    : ["Preview-контур недоступен."];

                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "archive_preview",
                    Title = "HARD ARCHIVE // PREVIEW",
                    Lines = archiveLines,
                };
                _statusMessage = "Контур Хард-Архива открыт для исследования (ADMIN).";
                AppendUiLog("[ARCHIVE] Preview-контур открыт.");
                WriteSessionLog("ARCHIVE", "Preview contour opened");
                break;

            case "mission_board":
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "mission_board",
                    Title = "ДОСКА ОПЕРАЦИЙ",
                    Lines =
                    [
                        "P01 // Эхо в стеке 9 // INDEXED",
                        "P02 // Несовместимый индекс // LOCKED",
                        "P03 // Пустой свидетель // SHADOWED",
                        MissionPageDebugAccessEnabled
                            ? "DEBUG // Generator Preview // ADMIN"
                            : "Protocol Zero required for launch.",
                    ],
                };
                _statusMessage = "Доска операций синхронизирована.";
                AppendUiLog("[MISSION] Доска операций открыта.");
                WriteSessionLog("MISSION", "Mission board opened");
                break;

            case "research_lab":
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "research_lab",
                    Title = "АНАЛИТИЧЕСКИЙ УЗЕЛ",
                    Lines =
                    [
                        "A0 anomaly scan active.",
                        "Состояние индекса: нестабильно.",
                        "Рекомендация: Verify -> Compare -> Resolve.",
                    ],
                };
                _statusMessage = "Исследовательский контур подключен.";
                AppendUiLog("[RESEARCH] Аналитический узел открыт.");
                WriteSessionLog("RESEARCH", "Research lab opened");
                break;

            case "archive_control":
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "archive_control",
                    Title = "ARCHIVE CONTROL",
                    Lines = UnlockHardArchivePreviewEnabled
                        ?
                        [
                            "Preview route unlocked.",
                            "EntryOctagon -> IndexVestibule -> StackRing",
                            "ReturnNode route is available.",
                        ]
                        :
                        [
                            "Hard Archive remains locked.",
                            "Complete first operation to authorize descent.",
                        ],
                };
                if (_bundle.HubRuntime.CurrentPhase >= HubRhythmPhase.OperationAccess)
                {
                    _hardArchivePartialUnlock = true;
                    UnlockArchivePreviewRoom("A0_ARCHIVE_CORRIDOR");
                    AppendUiLog("[ARCHIVE] Врата переведены в режим частичного доступа: открыт коридор допуска.");
                    WriteSessionLog("STATE", "HardArchiveGate=partial-unlock");
                    WriteSessionLog("ACT1", "Access corridor unlocked via archive_control");
                }
                _statusMessage = "Контур контроля архива обновлен.";
                AppendUiLog("[ARCHIVE] Контур контроля архива открыт.");
                WriteSessionLog("ARCHIVE", "Archive control opened");
                break;

            case "archive_corridor":
                _entryOctagonUnlocked = true;
                UnlockArchivePreviewRoom("A1_ENTRY_OCTAGON");
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "archive_corridor",
                    Title = "HARD ARCHIVE // ACCESS CORRIDOR",
                    Lines =
                    [
                        "Шлюз A-0 подтвержден.",
                        "Маршрут: Corridor -> Entry Octagon.",
                        "Рекомендация: проведите первичную разведку сектора.",
                    ],
                };
                _statusMessage = "Коридор доступа активен.";
                AppendUiLog("[ACT I] Коридор доступа открыт. Следующая точка: Entry Octagon.");
                WriteSessionLog("ACT1", "Access corridor visited; entry_octagon unlocked");
                break;

            case "entry_octagon":
                _indexVestibuleUnlocked = true;
                UnlockArchivePreviewRoom("A2_INDEX_VESTIBULE");
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "entry_octagon",
                    Title = "ENTRY OCTAGON",
                    Lines =
                    [
                        "Первичная архивная камера подтверждена.",
                        "Сигналы индекса направляют в Index Vestibule.",
                        "Стабильность сектора: LIMITED.",
                    ],
                };
                _statusMessage = "Entry Octagon исследован.";
                AppendUiLog("[ACT I] Entry Octagon пройден. Открой Index Vestibule.");
                WriteSessionLog("ACT1", "Entry Octagon visited; index_vestibule unlocked");
                break;

            case "index_vestibule":
                _researchRoomUnlocked = true;
                UnlockArchivePreviewRoom("A3_RESEARCH_ROUTE");
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "index_vestibule",
                    Title = "INDEX VESTIBULE",
                    Lines =
                    [
                        "Каталог маршрутов восстановлен частично.",
                        "Research Room 01 назначен как первая миссионная точка.",
                        "Задача: провести анализ аномального блока.",
                    ],
                };
                _statusMessage = "Index Vestibule синхронизирован.";
                AppendUiLog("[ACT I] Index Vestibule активирован. Открой Research Room 01.");
                WriteSessionLog("ACT1", "Index Vestibule visited; research_room_01 unlocked");
                break;

            case "research_room_01":
                _stackRingPreviewUnlocked = true;
                UnlockArchivePreviewRoom("A4_STACK_RING_PREVIEW");
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "research_room_01",
                    Title = "RESEARCH ROOM 01",
                    Lines =
                    [
                        "Аномальный сегмент локализован.",
                        "Первый акт: исследовательский контур подтвержден.",
                        "Открыт Stack Ring Preview для обзора масштаба архива.",
                    ],
                };
                _statusMessage = "Первая исследовательская миссия подтверждена.";
                AppendUiLog("[ACT I] Research Room 01 завершен. Доступен Stack Ring Preview.");
                WriteSessionLog("ACT1", "Research Room 01 visited; stack_ring_preview unlocked");
                break;

            case "stack_ring_preview":
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "stack_ring_preview",
                    Title = "STACK RING // PREVIEW",
                    Lines =
                    [
                        "Масштаб архива подтвержден: кольцевой контур активен.",
                        "Вертикальные переходы заблокированы до следующих операций.",
                        "Session 10 vertical slice: A0 -> Gate -> Entry -> Index -> Research READY.",
                    ],
                };
                _statusMessage = "Stack Ring Preview доступен.";
                AppendUiLog("[ACT I] Stack Ring Preview открыт: масштаб архива показан.");
                WriteSessionLog("ACT1", "Stack Ring Preview visited");
                break;

            case "tool_bench":
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "tool_bench",
                    Title = "TOOL BENCH",
                    Lines =
                    [
                        "Blueprint: Archive Address Inspector",
                        "Required fragments: Pattern + Parser + Verify",
                        "Assembly state: preview-only in Session 10.",
                    ],
                };
                _statusMessage = "Сборочный стол доступен.";
                AppendUiLog("[TECH] Сборочный стол: preview открыт.");
                WriteSessionLog("TECH", "Tool bench opened");
                break;

            case "commerce_desk":
                _activeTerminal = new TerminalScreen
                {
                    TerminalId = "commerce_desk",
                    Title = "СЕРВИСНЫЙ МОДУЛЬ",
                    Lines =
                    [
                        "Credits / Laun exchange node (simulated).",
                        "Utility modules improve analysis quality.",
                        "No pay-to-win unlocks in core progression.",
                    ],
                };
                _statusMessage = "Сервисный модуль активирован.";
                AppendUiLog("[COMMERCE] Сервисный модуль открыт.");
                WriteSessionLog("COMMERCE", "Service desk opened");
                break;
        }

            AppendUiLog($"[РЕЗУЛЬТАТ] {(_statusMessage ?? "Операция выполнена.")}");
            WriteSessionLog("RESULT", _statusMessage ?? "Operation completed");
    }

    private static Color ResolveContourColor(ContourRenderState state)
    {
        return state switch
        {
            ContourRenderState.Focused => Color.FromArgb(255, 225, 95),
            ContourRenderState.Used => Color.FromArgb(80, 200, 90),
            ContourRenderState.Locked => Color.FromArgb(170, 75, 75),
            _ => Color.FromArgb(95, 170, 255),
        };
    }

    private void RecordRenderRegistryTelemetry(long nowMs)
    {
        if (_bundle is null)
            return;

        if (nowMs - _lastRenderProbeMs < 4000)
            return;

        var mapped = 0;
        var fallback = 0;
        var fallbackItems = new List<string>();

        foreach (var obj in _bundle.HubRuntime.Zones.SelectMany(z => z.Objects))
        {
            var descriptor = ContourRenderRegistry.ResolveDescriptor(
                obj.ModelId,
                ContourRenderRegistry.GetInteractableFallbackSize(obj.InteractiveType));
            if (descriptor.UsedFallback)
            {
                fallback++;
                fallbackItems.Add($"{obj.Id}:{descriptor.FallbackReasonKey}");
            }
            else
            {
                mapped++;
            }
        }

        foreach (var node in ExtraNodes)
        {
            if (!IsExtraNodeAvailable(node.Id))
                continue;

            var descriptor = ContourRenderRegistry.ResolveDescriptor(
                node.ModelId,
                ContourRenderRegistry.GetExtraNodeFallbackSize(node.Id, node.ModelId));
            if (descriptor.UsedFallback)
            {
                fallback++;
                fallbackItems.Add($"{node.Id}:{descriptor.FallbackReasonKey}");
            }
            else
            {
                mapped++;
            }
        }

        var fallbackPreview = fallbackItems.Count == 0
            ? "none"
            : string.Join(",", fallbackItems.Take(10));

        var summary = $"mapped={mapped}; fallback={fallback}; fallbackItems={fallbackPreview}";
        if (!string.Equals(summary, _lastRenderSummary, StringComparison.Ordinal))
        {
            WriteSessionLog("RENDER", summary);
            _lastRenderSummary = summary;
        }

        _lastRenderProbeMs = nowMs;
    }

    private void RecordRenderPerformanceTelemetry(long nowMs)
    {
        if (nowMs - _lastRenderPerfProbeMs < 5000)
            return;

        if (_drawMsSamples > 0)
        {
            WriteSessionLog(
                "RENDER-PERF",
                FormattableString.Invariant(
                    $"drawMsLast={_lastDrawMs:F3}; drawMsAvg={_drawMsAverage:F3}; fps={_fps:F1}; interactables={_lastVisibleInteractables}; extraNodes={_lastVisibleExtraNodes}; fxSteam={_fxCapsuleSteamEnabled}; fxCore={_fxCoreHologramEnabled}; fxRoute={_fxArchiveRouteGuideEnabled}"));
        }

        _lastRenderPerfProbeMs = nowMs;
    }

    private void RecordBackgroundTelemetry()
    {
        if (_bundle is null)
            return;

        var phaseChanged = false;
        var objectiveChanged = false;
        var focusChanged = false;
        var terminalChanged = false;
        var cameraChanged = false;
        var protocolChanged = false;

        if (_bundle.HubRuntime.CurrentPhase != _lastLoggedPhase)
        {
            WriteSessionLog("STATE", $"Phase changed: {_lastLoggedPhase} -> {_bundle.HubRuntime.CurrentPhase}");
            _lastLoggedPhase = _bundle.HubRuntime.CurrentPhase;
            phaseChanged = true;
        }

        var activeObjectiveId = _bundle.ObjectiveTracker.GetActive()?.ObjectiveId;
        if (!string.Equals(activeObjectiveId, _lastLoggedObjectiveId, StringComparison.Ordinal))
        {
            WriteSessionLog("STATE", $"Objective changed: {_lastLoggedObjectiveId ?? "<none>"} -> {activeObjectiveId ?? "<none>"}");
            _lastLoggedObjectiveId = activeObjectiveId;
            objectiveChanged = true;
        }

        var currentFocus = _focusedObjectId ?? _focusedExtraId;
        if (!string.Equals(currentFocus, _lastLoggedFocus, StringComparison.Ordinal))
        {
            if (!string.IsNullOrWhiteSpace(currentFocus))
                WriteSessionLog("FOCUS", currentFocus);
            _lastLoggedFocus = currentFocus;
            focusChanged = true;
        }

        var currentTerminal = _activeTerminal?.TerminalId;
        if (!string.Equals(currentTerminal, _lastLoggedTerminalId, StringComparison.Ordinal))
        {
            WriteSessionLog("OVERLAY", $"terminal={(currentTerminal ?? "<closed>")}");
            _lastLoggedTerminalId = currentTerminal;
            terminalChanged = true;
        }

        if (_bundle.Session.Camera.ActiveMode != _lastLoggedCameraMode)
        {
            WriteSessionLog("CAMERA", $"mode={_bundle.Session.Camera.ActiveMode}");
            _lastLoggedCameraMode = _bundle.Session.Camera.ActiveMode;
            cameraChanged = true;
        }

        if (_bundle.PrologueTracker.IsProtocolZeroUnlocked != _lastLoggedProtocolZero)
        {
            WriteSessionLog("STATE", $"ProtocolZero={_bundle.PrologueTracker.IsProtocolZeroUnlocked}");
            _lastLoggedProtocolZero = _bundle.PrologueTracker.IsProtocolZeroUnlocked;
            protocolChanged = true;
        }

        if (!string.Equals(_statusMessage, _lastLoggedStatusMessage, StringComparison.Ordinal))
        {
            WriteSessionLog("STATUS", _statusMessage ?? "<none>");
            _lastLoggedStatusMessage = _statusMessage;
        }

        var chainCompleted = GetArchiveChainCompletedSteps();
        var chainNext = GetArchiveChainNextNodeId();
        if (chainCompleted != _lastArchiveChainCompleted || !string.Equals(chainNext, _lastArchiveChainNextNode, StringComparison.Ordinal))
        {
            WriteSessionLog("CHAIN", $"progress={chainCompleted}/{ArchiveEntryChain.OrderedNodes.Count}; next={chainNext ?? "<done>"}");
            _lastArchiveChainCompleted = chainCompleted;
            _lastArchiveChainNextNode = chainNext;
        }

        var nowMs = _clock.ElapsedMilliseconds;
        var p = _bundle.Player.Position;
        var moved = (p - _lastHeartbeatPos).Length() >= 0.45f;
        var forcedInterval = nowMs - _lastHeartbeatMs >= 2500;
        if (moved || phaseChanged || objectiveChanged || focusChanged || terminalChanged || cameraChanged || protocolChanged || forcedInterval)
        {
            WriteSessionLog(
                "TICK",
                FormattableString.Invariant(
                    $"phase={_bundle.HubRuntime.CurrentPhase}; objective={activeObjectiveId ?? "<none>"}; pos=({p.X:F2},{p.Y:F2},{p.Z:F2}); focus={currentFocus ?? "<none>"}; terminal={currentTerminal ?? "<closed>"}; playerYaw={_controllerState.PlayerYaw:F3}; cameraYaw={GetCurrentCameraYaw():F3}; orbitOffset={_controllerState.OrbitOffsetYaw:F3}; uiCapture={_uiInputCaptureActive}; pauseMenu={IsPauseMenuOpen}; profile={_activeControlProfileId}; scheme={_controlProfile.SchemeId}"));
            _lastHeartbeatMs = nowMs;
            _lastHeartbeatPos = p;
        }

        if (nowMs - _lastIntegrationProbeMs >= 3000)
        {
            var objectiveText = _objectiveHud?.DisplayText ?? "<n/a>";
            WriteSessionLog(
                "INTEGRATION",
                FormattableString.Invariant(
                    $"bundle={_bundle is not null}; hints={_hints is not null}; objectiveHud={_objectiveHud is not null}; dialoguePlaying={_bundle!.DialoguePlayer.IsPlaying}; terminalOpen={_activeTerminal is not null}; missionSlots={_missionSlots.Count}; archiveRooms={_archivePreviewRooms.Count}; chainProgress={chainCompleted}/{ArchiveEntryChain.OrderedNodes.Count}; chainNext={chainNext ?? "<done>"}; objectiveText={objectiveText}; fps={_fps:F1}"));
            _lastIntegrationProbeMs = nowMs;
        }

        RecordRenderRegistryTelemetry(nowMs);
        RecordRenderPerformanceTelemetry(nowMs);
    }

    private void EmitImperativeDirective(bool force)
    {
        if (_bundle is null)
            return;

        var objectiveId = _bundle.ObjectiveTracker.GetActive()?.ObjectiveId ?? "<none>";
        var tone = ResolveSceneTone(_bundle.HubRuntime.CurrentPhase);
        var imperative = ResolveImperativeTask(_bundle.HubRuntime.CurrentPhase, objectiveId);
        var key = $"{_bundle.HubRuntime.CurrentPhase}|{objectiveId}|{imperative}";

        if (!force && string.Equals(key, _lastImperativeKey, StringComparison.Ordinal))
            return;

        if (!string.Equals(tone.Title, _lastSceneTitle, StringComparison.Ordinal))
        {
            AppendUiLog($"[SCENE] {tone.Title} -> {tone.Subtitle}");
            WriteSessionLog("SCENE", $"{tone.Title} | {tone.Subtitle}");
            _lastSceneTitle = tone.Title;
        }

        AppendUiLog($"[₫ ИМПЕРАТИВ] Сделай: {imperative}");
        WriteSessionLog("IMPERATIVE", imperative);
        _lastImperativeKey = key;
    }

    private void AnalyzePathingAndBackgroundIssues(Vec3 beforeFramePos, float dt, bool blockedByOverlay)
    {
        if (_bundle is null)
            return;

        var movementIntent = !blockedByOverlay &&
            (IsBoundHeld(_inputBindings.MoveForward) ||
             IsBoundHeld(_inputBindings.MoveBackward) ||
             IsBoundHeld(_inputBindings.MoveLeft) ||
             IsBoundHeld(_inputBindings.MoveRight));
        var movedDistance = (_bundle.Player.Position - beforeFramePos).Length();

        if (movementIntent && movedDistance < 0.012f)
            _blockedMovementMs += dt * 1000d;
        else
            _blockedMovementMs = 0d;

        if (_blockedMovementMs >= 1400d && !_isPathBlocked)
        {
            _isPathBlocked = true;
            var focus = _focusedObjectName ?? _focusedExtraName ?? "неизвестный объект";
            AppendUiLog($"[BOT] Путь может быть прегражден около: {focus}. Вопрос: удалось пройти? (ответь /ok path или /fail path причина)");
            WriteSessionLog("PATHBLOCK", $"suspected=true; focus={focus}; phase={_bundle.HubRuntime.CurrentPhase}");
            WriteSessionLog("BOT-QUESTION", "path: удалось пройти препятствие? /ok path или /fail path <причина>");
        }

        if (_isPathBlocked && movedDistance >= 0.12f)
        {
            _isPathBlocked = false;
            _blockedMovementMs = 0d;
            AppendUiLog("[BOT] Движение восстановлено. Подтверди результат: /ok path или /fail path причина");
            WriteSessionLog("PATHBLOCK", "suspected=false; movement restored");
        }

        _lastPlayerPos = _bundle.Player.Position;
    }

    private void RunSmartTestBot()
    {
        if (_bundle is null)
            return;

        var rank = PhaseRank(_bundle.HubRuntime.CurrentPhase);
        if (rank > _lastCheckpointPhaseRank)
        {
            for (var step = _lastCheckpointPhaseRank + 1; step <= rank; step++)
            {
                var title = step switch
                {
                    1 => "Выход из капсулы",
                    2 => "Биометрия пройдена",
                    3 => "Снабжение получено",
                    4 => "Дрон активирован",
                    5 => "C.O.R.E. активирован",
                    _ => "Новый этап",
                };

                AppendUiLog($"[BOT] Checkpoint PASS: {title}. Вопрос: удалось/нет? (ответь /ok step{step} или /fail step{step} причина)");
                WriteSessionLog("BOT-CHECKPOINT", $"step={step}; title={title}; status=pass");
                WriteSessionLog("BOT-QUESTION", $"step{step}: удалось? /ok step{step} или /fail step{step} <причина>");
            }

            _lastCheckpointPhaseRank = rank;
        }

        if (!_operationQuestionIssued && _bundle.HubRuntime.CurrentPhase >= HubRhythmPhase.OperationAccess)
        {
            _operationQuestionIssued = true;
            AppendUiLog("[BOT] Этап операций открыт. Вопрос: удалось протестировать op/research/gallery/archive? (ответь /ok ops или /fail ops причина)");
            WriteSessionLog("BOT-QUESTION", "ops: удалось протестировать operation nodes? /ok ops или /fail ops <причина>");
        }

        if (!_actOneFlowQuestionIssued && _hardArchivePartialUnlock)
        {
            _actOneFlowQuestionIssued = true;
            AppendUiLog("[BOT] Первый акт открыт. Вопрос: прошел маршрут corridor -> octagon -> vestibule? (ответь /ok act1flow или /fail act1flow причина)");
            WriteSessionLog("BOT-QUESTION", "act1flow: corridor/octagon/vestibule пройден? /ok act1flow или /fail act1flow <причина>");
        }

        if (!_actOneCompleteQuestionIssued && _stackRingPreviewUnlocked)
        {
            _actOneCompleteQuestionIssued = true;
            AppendUiLog("[BOT] Вертикальный срез Session 10 завершен. Вопрос: подтверждаешь готовность к сборке? (ответь /ok session10 или /fail session10 причина)");
            WriteSessionLog("BOT-QUESTION", "session10: vertical slice complete? /ok session10 или /fail session10 <причина>");
        }
    }

    private void UnlockArchivePreviewRoom(string roomId)
    {
        if (_archivePreviewRooms.Contains(roomId, StringComparer.OrdinalIgnoreCase))
            return;

        _archivePreviewRooms = [.. _archivePreviewRooms, roomId];
        WriteSessionLog("ARCHIVE", $"Preview room unlocked: {roomId}");
    }

    private static int PhaseRank(HubRhythmPhase phase)
    {
        return phase switch
        {
            HubRhythmPhase.Awakening => 0,
            HubRhythmPhase.Identification => 1,
            HubRhythmPhase.Provisioning => 2,
            HubRhythmPhase.DroneContact => 3,
            HubRhythmPhase.Activation => 4,
            HubRhythmPhase.OperationAccess => 5,
            _ => 0,
        };
    }

    private string ResolveImperativeTask(HubRhythmPhase phase, string objectiveId)
    {
        if (phase >= HubRhythmPhase.OperationAccess)
        {
            if (!_hardArchivePartialUnlock)
                return "зайди в archive_control и открой коридор допуска в Хард-Архив.";

            if (!_entryOctagonUnlocked)
                return "пройди в archive_corridor и активируй переход в Entry Octagon.";

            if (!_indexVestibuleUnlocked)
                return "исследуй entry_octagon и открой Index Vestibule.";

            if (!_researchRoomUnlocked)
                return "заверши синхронизацию index_vestibule и открой Research Room 01.";

            if (!_stackRingPreviewUnlocked)
                return "выполни исследование в research_room_01 и открой Stack Ring Preview.";

            return "проверь stack_ring_preview и зафиксируй итоги вертикального среза Session 10.";
        }

        if (objectiveId.Equals("UI_OBJ_PROLOGUE_DONE", StringComparison.Ordinal))
            return "открой mission_board, исследуй archive_preview и зафиксируй заметку по качеству сцены.";

        return phase switch
        {
            HubRhythmPhase.Awakening => "выйди из капсулы: подойди к capsule_exit и нажми E.",
            HubRhythmPhase.Identification => "пройди биометрию: у bio_scanner нажми E и дождись подтверждения.",
            HubRhythmPhase.Provisioning => "забери комплект: у supply_terminal нажми E и проверь лут в HUD.",
            HubRhythmPhase.DroneContact => "активируй дрон: подойди к drone_dock и нажми E.",
            HubRhythmPhase.Activation => "запусти C.O.R.E.: взаимодействуй с core_console.",
            HubRhythmPhase.OperationAccess => "протестируй узлы operation/research/gallery/archive_gate и зафиксируй замечания.",
            _ => "продолжай маршрут миссии и записывай выводы в заметки справа.",
        };
    }

    private static (Color Top, Color Bottom, Color Glow, Color Fog, string Title, string Subtitle) ResolveSceneTone(HubRhythmPhase phase)
    {
        return phase switch
        {
            HubRhythmPhase.Awakening => (Color.FromArgb(14, 24, 48), Color.FromArgb(8, 11, 22), Color.FromArgb(88, 122, 210), Color.FromArgb(20, 36, 66), "SCN-A0 // Awakening", "Капсула и первичное пробуждение"),
            HubRhythmPhase.Identification => (Color.FromArgb(20, 36, 58), Color.FromArgb(10, 16, 28), Color.FromArgb(84, 186, 230), Color.FromArgb(30, 66, 88), "SCN-A1 // Identification", "Биометрический верификатор"),
            HubRhythmPhase.Provisioning => (Color.FromArgb(30, 40, 54), Color.FromArgb(14, 18, 26), Color.FromArgb(120, 188, 255), Color.FromArgb(52, 80, 112), "SCN-A2 // Provisioning", "Логистический модуль и сбор снаряжения"),
            HubRhythmPhase.DroneContact => (Color.FromArgb(18, 40, 52), Color.FromArgb(9, 18, 24), Color.FromArgb(80, 232, 200), Color.FromArgb(28, 90, 92), "SCN-A3 // Drone Contact", "Активация помощника-дрона"),
            HubRhythmPhase.Activation => (Color.FromArgb(44, 34, 60), Color.FromArgb(18, 14, 28), Color.FromArgb(220, 146, 252), Color.FromArgb(84, 62, 108), "SCN-A4 // Core Activation", "Запуск C.O.R.E. и синхронизация"),
            HubRhythmPhase.OperationAccess => (Color.FromArgb(56, 36, 34), Color.FromArgb(22, 14, 14), Color.FromArgb(255, 176, 112), Color.FromArgb(110, 62, 50), "SCN-A5 // Operations Access", "Доступ к операциям и контуру архива"),
            _ => (Color.FromArgb(18, 22, 34), Color.FromArgb(8, 10, 18), Color.FromArgb(170, 200, 240), Color.FromArgb(34, 46, 74), "SCN-A0 // Initiation", "Основной игровой узел"),
        };
    }

    private void WriteSessionLog(string category, string message)
    {
        if (string.IsNullOrWhiteSpace(_sessionLogPath) && string.IsNullOrWhiteSpace(_sessionCurrentLogPath))
            return;

        try
        {
            var line = $"[{DateTimeOffset.Now:O}] [{category}] {message}{Environment.NewLine}";
            AppendLineToLogPath(_sessionLogPath, line);
            AppendLineToLogPath(_sessionCurrentLogPath, line);
        }
        catch (Exception ex)
        {
            var fallbackPath = Path.Combine(AppContext.BaseDirectory, "runtime", "desktop-session-error.log");
            File.AppendAllText(
                fallbackPath,
                $"[{DateTimeOffset.Now:O}] WriteSessionLog failed: {ex.Message}{Environment.NewLine}",
                Encoding.UTF8);
        }
    }

    private bool ConsumePressed(Keys key)
    {
        if (key == Keys.None)
            return false;

        if (_keysPressed.Contains(key))
        {
            _keysPressed.Remove(key);
            return true;
        }
        return false;
    }

    private bool IsBoundHeld(Keys key)
    {
        return key != Keys.None && _keysDown.Contains(key);
    }

    private bool IsGameplayBoundKey(Keys key)
    {
        return key != Keys.None &&
            (key == _inputBindings.MoveForward ||
             key == _inputBindings.MoveBackward ||
             key == _inputBindings.MoveLeft ||
             key == _inputBindings.MoveRight ||
             key == _inputBindings.TurnLeft ||
             key == _inputBindings.TurnRight ||
             key == _inputBindings.Interact ||
             key == _inputBindings.CameraToggle ||
             key == _inputBindings.OrbitModifier ||
             key == _inputBindings.DialogueAdvance ||
             key == _inputBindings.PauseMenu ||
             key == _inputBindings.ResetSession);
    }

    private void ConsumeMouseDelta(out float deltaX, out float deltaY)
    {
        deltaX = _mouseDeltaX;
        deltaY = _mouseDeltaY;
        _mouseDeltaX = 0f;
        _mouseDeltaY = 0f;
    }

    private bool IsMouseButtonActive(MouseControlButton button)
    {
        return button switch
        {
            MouseControlButton.Left => _leftMouseDown,
            MouseControlButton.Right => _rightMouseDown,
            MouseControlButton.Middle => _middleMouseDown,
            _ => false,
        };
    }

    private static float Dot(Vec3 a, Vec3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    private static Vec3 Cross(Vec3 a, Vec3 b) =>
        new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

    private static Vec3 RotateY(Vec3 v, float angle)
    {
        var c = MathF.Cos(angle);
        var s = MathF.Sin(angle);
        return new Vec3(v.X * c + v.Z * s, v.Y, -v.X * s + v.Z * c);
    }

    private float GetCurrentCameraYaw()
    {
        return _controllerState.CurrentCameraYaw;
    }

    private static string ResolveBuildStamp()
    {
        try
        {
            var exePath = Path.Combine(AppContext.BaseDirectory, "BabylonArchiveCore.Desktop.exe");
            if (!File.Exists(exePath))
                return "unknown";

            var stamp = File.GetLastWriteTime(exePath);
            return stamp.ToString("yyyy-MM-dd HH:mm:ss");
        }
        catch
        {
            return "unknown";
        }
    }

    private static void DrawEdge(Graphics g, Pen pen, PointF?[] p, int i, int j)
    {
        g.DrawLine(pen, p[i]!.Value, p[j]!.Value);
    }

    private void AppendUiLog(string line)
    {
        _logBox.AppendText(line + Environment.NewLine);
        _logBox.SelectionStart = _logBox.TextLength;
        _logBox.ScrollToCaret();
    }

    private void SaveOperatorNote()
    {
        var note = _noteInput.Text.Trim();
        if (string.IsNullOrWhiteSpace(note))
            return;

        if (TryHandleBotResponse(note))
        {
            _noteInput.Clear();
            _noteInput.Focus();
            return;
        }

        var timestamp = DateTimeOffset.Now;
        AppendUiLog($"[ЗАМЕТКА] Сохранено: {note}");
        WriteSessionLog("NOTE", note);

        if (!string.IsNullOrWhiteSpace(_notesLogPath))
        {
            try
            {
                var line = $"[{timestamp:O}] {note}{Environment.NewLine}";
                File.AppendAllText(_notesLogPath, line, Encoding.UTF8);
            }
            catch
            {
                AppendUiLog("[ОШИБКА] Не удалось сохранить заметку в файл.");
            }
        }

        _noteInput.Clear();
        _noteInput.Focus();
    }

    private bool TryHandleBotResponse(string note)
    {
        if (note.StartsWith("/ok ", StringComparison.OrdinalIgnoreCase))
        {
            var label = note[4..].Trim();
            if (string.IsNullOrWhiteSpace(label))
                label = "unknown";

            AppendUiLog($"[BOT] Подтверждено: {label} -> УДАЛОСЬ");
            WriteSessionLog("BOT-ANSWER", $"{label}=ok");
            return true;
        }

        if (note.StartsWith("/fail ", StringComparison.OrdinalIgnoreCase))
        {
            var payload = note[6..].Trim();
            var firstSpace = payload.IndexOf(' ');
            string label;
            string reason;
            if (firstSpace < 0)
            {
                label = string.IsNullOrWhiteSpace(payload) ? "unknown" : payload;
                reason = "не указана";
            }
            else
            {
                label = payload[..firstSpace].Trim();
                reason = payload[(firstSpace + 1)..].Trim();
                if (string.IsNullOrWhiteSpace(reason))
                    reason = "не указана";
            }

            AppendUiLog($"[BOT] Зафиксирован сбой: {label} -> {reason}");
            WriteSessionLog("BOT-ANSWER", $"{label}=fail; reason={reason}");
            WriteSessionLog("BOT-ISSUE", $"checkpoint={label}; reason={reason}; phase={_bundle?.HubRuntime.CurrentPhase}");
            return true;
        }

        return false;
    }

    private void SetUiInputCapture(bool active, string source)
    {
        if (_uiInputCaptureActive == active)
            return;

        _uiInputCaptureActive = active;
        _controllerState.IsUiInputCaptured = active;
        _keysDown.Clear();
        _keysPressed.Clear();

        if (active)
        {
            if (string.Equals(source, "pause-menu", StringComparison.Ordinal))
                AppendUiLog("[MENU] Пауза активна: игровой ввод остановлен.");
            else
                AppendUiLog("[CONTROL] Фокус ввода активен: управление персонажем временно заблокировано.");

            WriteSessionLog("CONTROL", $"uiInputCapture=on; source={source}");
            _statusMessage = string.Equals(source, "pause-menu", StringComparison.Ordinal)
                ? "Пауза: выберите действие в ESC-меню."
                : "Режим ввода: клавиатура направлена в игровой лог. Кликните по игровому окну для возврата управления.";
            return;
        }

        _viewport.Focus();

        if (string.Equals(source, "pause-menu", StringComparison.Ordinal))
            AppendUiLog("[MENU] Пауза снята.");
        else
            AppendUiLog("[CONTROL] Фокус управления возвращен в игровое окно.");

        WriteSessionLog("CONTROL", $"uiInputCapture=off; source={source}");
        _statusMessage = "Управление возвращено: активны текущие бинды и мышь профиля.";
    }

    private void EnsureSessionLogInitialized()
    {
        if (string.IsNullOrWhiteSpace(_sessionLogPath) && string.IsNullOrWhiteSpace(_sessionCurrentLogPath))
            return;

        try
        {
            var mainDir = Path.GetDirectoryName(_sessionLogPath);
            if (!string.IsNullOrWhiteSpace(mainDir))
                Directory.CreateDirectory(mainDir);

            var currentDir = Path.GetDirectoryName(_sessionCurrentLogPath);
            if (!string.IsNullOrWhiteSpace(currentDir))
                Directory.CreateDirectory(currentDir);

            var marker = $"[{DateTimeOffset.Now:O}] [SYSTEM] Session log initialized.{Environment.NewLine}";
            AppendLineToLogPath(_sessionLogPath, marker);
            if (!string.IsNullOrWhiteSpace(_sessionCurrentLogPath))
            {
                File.WriteAllText(_sessionCurrentLogPath, string.Empty, Encoding.UTF8);
                AppendLineToLogPath(_sessionCurrentLogPath, marker);
            }
        }
        catch (Exception ex)
        {
            AppendUiLog($"[ОШИБКА] Не удалось инициализировать session log: {ex.Message}");
            var fallbackPath = Path.Combine(AppContext.BaseDirectory, "runtime", "desktop-session-error.log");
            File.AppendAllText(
                fallbackPath,
                $"[{DateTimeOffset.Now:O}] EnsureSessionLogInitialized failed: {ex}{Environment.NewLine}",
                Encoding.UTF8);
        }
    }

    private static void AppendLineToLogPath(string? path, string line)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        using var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(line);
    }

    private void EnsureAdminFlagsInitialized()
    {
        _adminUnlockAllRooms = _runtimeProfile.UnlockAllRooms;
        _adminUnlockAllTerminals = _runtimeProfile.UnlockAllTerminals;
        _adminUnlockHardArchivePreview = _runtimeProfile.UnlockHardArchivePreview;
        _adminEnablePurchaseSimulation = _runtimeProfile.EnablePurchaseSimulation;
        _adminEnableMissionPageDebugAccess = _runtimeProfile.EnableMissionPageDebugAccess;
    }

    private DesktopMenuOptions BuildDefaultDesktopOptions()
    {
        var modernPreset = ControlProfilePresetCatalog.GetBuiltInProfileOrDefault(ControlProfilePresetCatalog.ModernThirdPersonId);
        var presets = ControlProfilePresetCatalog.BuiltInPresets
            .Select(p => new DesktopControlProfileEntry
            {
                Id = p.Id,
                DisplayName = p.DisplayName,
                IsCustom = p.IsCustom,
                Profile = ControlProfilePresetCatalog.Normalize(p.Profile),
            })
            .ToList();

        return new DesktopMenuOptions
        {
            SchemaVersion = 3,
            ActiveControlProfileId = ControlProfilePresetCatalog.ModernThirdPersonId,
            DefaultControlProfileId = ControlProfilePresetCatalog.ModernThirdPersonId,
            ControlProfiles = presets,
            InputBindings = DesktopInputBindings.Default,
            InvertOrbitHorizontal = modernPreset.InvertOrbitHorizontal,
            InvertOrbitVertical = modernPreset.InvertOrbitVertical,
            OrbitSensitivity = modernPreset.OrbitSensitivity,
            OrbitSensitivityVertical = modernPreset.OrbitSensitivityVertical,
            CameraSmoothing = modernPreset.CameraSmoothing,
            MaxOrbitOffsetYaw = modernPreset.MaxOrbitOffsetYaw,
            MaxOrbitVerticalOffset = modernPreset.MaxOrbitVerticalOffset,
            FxCapsuleSteamEnabled = true,
            FxCoreHologramEnabled = true,
            FxArchiveRouteGuideEnabled = true,
            ShowExtendedHudTelemetry = true,
            UnlockAllRooms = _runtimeProfile.UnlockAllRooms,
            UnlockAllTerminals = _runtimeProfile.UnlockAllTerminals,
            UnlockHardArchivePreview = _runtimeProfile.UnlockHardArchivePreview,
            EnablePurchaseSimulation = _runtimeProfile.EnablePurchaseSimulation,
            EnableMissionPageDebugAccess = _runtimeProfile.EnableMissionPageDebugAccess,
        };
    }

    private void ApplyDesktopOptions(DesktopMenuOptions options)
    {
        var normalized = NormalizeDesktopOptions(options);

        _controlProfiles.Clear();
        foreach (var entry in normalized.ControlProfiles)
        {
            _controlProfiles.Add(new DesktopControlProfileEntry
            {
                Id = entry.Id,
                DisplayName = entry.DisplayName,
                IsCustom = entry.IsCustom,
                Profile = ControlProfilePresetCatalog.Normalize(entry.Profile),
            });
        }

        _activeControlProfileId = normalized.ActiveControlProfileId;
        _defaultControlProfileId = normalized.DefaultControlProfileId;
        _inputBindings = normalized.InputBindings;
        EnsureActiveControlProfile();

        _fxCapsuleSteamEnabled = normalized.FxCapsuleSteamEnabled;
        _fxCoreHologramEnabled = normalized.FxCoreHologramEnabled;
        _fxArchiveRouteGuideEnabled = normalized.FxArchiveRouteGuideEnabled;
        _showExtendedHudTelemetry = normalized.ShowExtendedHudTelemetry;
        _adminUnlockAllRooms = normalized.UnlockAllRooms;
        _adminUnlockAllTerminals = normalized.UnlockAllTerminals;
        _adminUnlockHardArchivePreview = normalized.UnlockHardArchivePreview;
        _adminEnablePurchaseSimulation = normalized.EnablePurchaseSimulation;
        _adminEnableMissionPageDebugAccess = normalized.EnableMissionPageDebugAccess;

        var existingCustom = _controlProfiles.Count(p => p.IsCustom);
        _customProfileCounter = Math.Max(1, existingCustom + 1);
    }

    private DesktopMenuOptions BuildDesktopOptionsSnapshot()
    {
        SaveActiveControlProfileToCollection();

        return new DesktopMenuOptions
        {
            SchemaVersion = 3,
            ActiveControlProfileId = _activeControlProfileId,
            DefaultControlProfileId = _defaultControlProfileId,
            ControlProfiles = _controlProfiles
                .Select(p => new DesktopControlProfileEntry
                {
                    Id = p.Id,
                    DisplayName = p.DisplayName,
                    IsCustom = p.IsCustom,
                    Profile = ControlProfilePresetCatalog.Normalize(p.Profile),
                })
                .ToList(),
            InputBindings = _inputBindings,
            InvertOrbitHorizontal = _controlProfile.InvertOrbitHorizontal,
            InvertOrbitVertical = _controlProfile.InvertOrbitVertical,
            OrbitSensitivity = _controlProfile.OrbitSensitivity,
            OrbitSensitivityVertical = _controlProfile.OrbitSensitivityVertical,
            CameraSmoothing = _controlProfile.CameraSmoothing,
            MaxOrbitOffsetYaw = _controlProfile.MaxOrbitOffsetYaw,
            MaxOrbitVerticalOffset = _controlProfile.MaxOrbitVerticalOffset,
            FxCapsuleSteamEnabled = _fxCapsuleSteamEnabled,
            FxCoreHologramEnabled = _fxCoreHologramEnabled,
            FxArchiveRouteGuideEnabled = _fxArchiveRouteGuideEnabled,
            ShowExtendedHudTelemetry = _showExtendedHudTelemetry,
            UnlockAllRooms = _adminUnlockAllRooms,
            UnlockAllTerminals = _adminUnlockAllTerminals,
            UnlockHardArchivePreview = _adminUnlockHardArchivePreview,
            EnablePurchaseSimulation = _adminEnablePurchaseSimulation,
            EnableMissionPageDebugAccess = _adminEnableMissionPageDebugAccess,
        };
    }

    private void PersistDesktopOptions()
    {
        if (string.IsNullOrWhiteSpace(_optionsPath))
            return;

        _menuOptionsStore.Save(_optionsPath, BuildDesktopOptionsSnapshot());
        WriteSessionLog("SETTINGS", "desktop-options persisted");
    }

    private DesktopMenuOptions NormalizeDesktopOptions(DesktopMenuOptions options)
    {
        var normalizedProfiles = new List<DesktopControlProfileEntry>();

        if (options.ControlProfiles.Count == 0)
        {
            foreach (var preset in ControlProfilePresetCatalog.BuiltInPresets)
            {
                var profile = ControlProfilePresetCatalog.Normalize(preset.Profile);
                if (preset.Id.Equals(ControlProfilePresetCatalog.ModernThirdPersonId, StringComparison.Ordinal))
                {
                    profile = ControlProfilePresetCatalog.Normalize(profile with
                    {
                        InvertOrbitHorizontal = options.InvertOrbitHorizontal,
                        InvertOrbitVertical = options.InvertOrbitVertical,
                        OrbitSensitivity = options.OrbitSensitivity,
                        OrbitSensitivityVertical = options.OrbitSensitivityVertical,
                        CameraSmoothing = options.CameraSmoothing,
                        MaxOrbitOffsetYaw = options.MaxOrbitOffsetYaw,
                        MaxOrbitVerticalOffset = options.MaxOrbitVerticalOffset,
                    });
                }

                normalizedProfiles.Add(new DesktopControlProfileEntry
                {
                    Id = preset.Id,
                    DisplayName = preset.DisplayName,
                    IsCustom = preset.IsCustom,
                    Profile = profile,
                });
            }
        }
        else
        {
            foreach (var entry in options.ControlProfiles)
            {
                if (string.IsNullOrWhiteSpace(entry.Id))
                    continue;

                normalizedProfiles.Add(new DesktopControlProfileEntry
                {
                    Id = entry.Id.Trim(),
                    DisplayName = string.IsNullOrWhiteSpace(entry.DisplayName) ? entry.Id.Trim() : entry.DisplayName,
                    IsCustom = entry.IsCustom,
                    Profile = ControlProfilePresetCatalog.Normalize(entry.Profile),
                });
            }

            foreach (var preset in ControlProfilePresetCatalog.BuiltInPresets)
            {
                if (normalizedProfiles.Any(p => p.Id.Equals(preset.Id, StringComparison.Ordinal)))
                    continue;

                normalizedProfiles.Add(new DesktopControlProfileEntry
                {
                    Id = preset.Id,
                    DisplayName = preset.DisplayName,
                    IsCustom = preset.IsCustom,
                    Profile = ControlProfilePresetCatalog.Normalize(preset.Profile),
                });
            }
        }

        if (normalizedProfiles.Count == 0)
        {
            var fallback = ControlProfilePresetCatalog.BuiltInPresets[0];
            normalizedProfiles.Add(new DesktopControlProfileEntry
            {
                Id = fallback.Id,
                DisplayName = fallback.DisplayName,
                IsCustom = fallback.IsCustom,
                Profile = ControlProfilePresetCatalog.Normalize(fallback.Profile),
            });
        }

        var activeId = string.IsNullOrWhiteSpace(options.ActiveControlProfileId)
            ? ControlProfilePresetCatalog.ModernThirdPersonId
            : options.ActiveControlProfileId;
        if (!normalizedProfiles.Any(p => p.Id.Equals(activeId, StringComparison.Ordinal)))
            activeId = normalizedProfiles[0].Id;

        var defaultId = string.IsNullOrWhiteSpace(options.DefaultControlProfileId)
            ? activeId
            : options.DefaultControlProfileId;
        if (!normalizedProfiles.Any(p => p.Id.Equals(defaultId, StringComparison.Ordinal)))
            defaultId = activeId;

        var bindings = options.InputBindings ?? DesktopInputBindings.Default;

        return new DesktopMenuOptions
        {
            SchemaVersion = Math.Max(options.SchemaVersion, 3),
            ActiveControlProfileId = activeId,
            DefaultControlProfileId = defaultId,
            ControlProfiles = normalizedProfiles,
            InputBindings = bindings,
            InvertOrbitHorizontal = options.InvertOrbitHorizontal,
            InvertOrbitVertical = options.InvertOrbitVertical,
            OrbitSensitivity = options.OrbitSensitivity,
            OrbitSensitivityVertical = options.OrbitSensitivityVertical,
            CameraSmoothing = options.CameraSmoothing,
            MaxOrbitOffsetYaw = options.MaxOrbitOffsetYaw,
            MaxOrbitVerticalOffset = options.MaxOrbitVerticalOffset,
            FxCapsuleSteamEnabled = options.FxCapsuleSteamEnabled,
            FxCoreHologramEnabled = options.FxCoreHologramEnabled,
            FxArchiveRouteGuideEnabled = options.FxArchiveRouteGuideEnabled,
            ShowExtendedHudTelemetry = options.ShowExtendedHudTelemetry,
            UnlockAllRooms = options.UnlockAllRooms,
            UnlockAllTerminals = options.UnlockAllTerminals,
            UnlockHardArchivePreview = options.UnlockHardArchivePreview,
            EnablePurchaseSimulation = options.EnablePurchaseSimulation,
            EnableMissionPageDebugAccess = options.EnableMissionPageDebugAccess,
        };
    }

    private void EnsureActiveControlProfile()
    {
        if (_controlProfiles.Count == 0)
        {
            var fallback = ControlProfilePresetCatalog.BuiltInPresets[0];
            _controlProfiles.Add(new DesktopControlProfileEntry
            {
                Id = fallback.Id,
                DisplayName = fallback.DisplayName,
                IsCustom = fallback.IsCustom,
                Profile = ControlProfilePresetCatalog.Normalize(fallback.Profile),
            });
            _activeControlProfileId = fallback.Id;
        }

        var active = _controlProfiles.FirstOrDefault(p => p.Id.Equals(_activeControlProfileId, StringComparison.Ordinal));
        if (active is null)
        {
            active = _controlProfiles[0];
            _activeControlProfileId = active.Id;
        }

        if (!_controlProfiles.Any(p => p.Id.Equals(_defaultControlProfileId, StringComparison.Ordinal)))
            _defaultControlProfileId = _activeControlProfileId;

        _controlProfile = ControlProfilePresetCatalog.Normalize(active.Profile);
    }

    private void SaveActiveControlProfileToCollection()
    {
        var index = _controlProfiles.FindIndex(p => p.Id.Equals(_activeControlProfileId, StringComparison.Ordinal));
        if (index < 0)
            return;

        var current = _controlProfiles[index];
        _controlProfiles[index] = new DesktopControlProfileEntry
        {
            Id = current.Id,
            DisplayName = current.DisplayName,
            IsCustom = current.IsCustom,
            Profile = ControlProfilePresetCatalog.Normalize(_controlProfile),
        };
    }

    private void SetActiveControlProfile(string profileId)
    {
        if (string.IsNullOrWhiteSpace(profileId))
            return;

        SaveActiveControlProfileToCollection();

        var target = _controlProfiles.FirstOrDefault(p => p.Id.Equals(profileId, StringComparison.Ordinal));
        if (target is null)
            return;

        _activeControlProfileId = target.Id;
        _controlProfile = ControlProfilePresetCatalog.Normalize(target.Profile);
        _statusMessage = $"Профиль управления активирован: {target.DisplayName}";
        WriteSessionLog("CONTROL", $"activeProfile={target.Id}; scheme={_controlProfile.SchemeId}");
    }

    private void CreateCustomProfileFromActive()
    {
        SaveActiveControlProfileToCollection();

        var id = $"custom-profile-{_customProfileCounter:00}";
        _customProfileCounter++;
        var name = $"Custom {_customProfileCounter - 1:00}";

        var clone = new DesktopControlProfileEntry
        {
            Id = id,
            DisplayName = name,
            IsCustom = true,
            Profile = ControlProfilePresetCatalog.Normalize(_controlProfile),
        };

        _controlProfiles.Add(clone);
        SetActiveControlProfile(clone.Id);
        PersistDesktopOptions();
    }

    private void DeleteActiveCustomProfile()
    {
        var current = _controlProfiles.FirstOrDefault(p => p.Id.Equals(_activeControlProfileId, StringComparison.Ordinal));
        if (current is null || !current.IsCustom)
            return;

        _controlProfiles.RemoveAll(p => p.Id.Equals(_activeControlProfileId, StringComparison.Ordinal));
        _activeControlProfileId = ControlProfilePresetCatalog.ModernThirdPersonId;
        EnsureActiveControlProfile();
        PersistDesktopOptions();
    }

    private void ResetActiveProfileToPresetDefaults()
    {
        var current = _controlProfiles.FirstOrDefault(p => p.Id.Equals(_activeControlProfileId, StringComparison.Ordinal));
        if (current is null)
            return;

        var preset = ControlProfilePresetCatalog.BuiltInPresets
            .FirstOrDefault(p => p.Profile.SchemeId == _controlProfile.SchemeId);
        var resetProfile = preset?.Profile
            ?? ControlProfilePresetCatalog.GetBuiltInProfileOrDefault(ControlProfilePresetCatalog.ModernThirdPersonId);

        _controlProfile = ControlProfilePresetCatalog.Normalize(resetProfile);
        SaveActiveControlProfileToCollection();
        PersistDesktopOptions();
    }

    private void SetActiveProfileAsDefault()
    {
        _defaultControlProfileId = _activeControlProfileId;
        PersistDesktopOptions();
        AppendUiLog($"[SETTINGS] Профиль по умолчанию: {_defaultControlProfileId}");
        WriteSessionLog("CONTROL", $"defaultProfile={_defaultControlProfileId}; source=menu");
    }

    private void ApplyDefaultProfileFromOperatorNotes()
    {
        if (string.IsNullOrWhiteSpace(_notesLogPath) || !File.Exists(_notesLogPath))
            return;

        var lines = File.ReadAllLines(_notesLogPath, Encoding.UTF8);
        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i];
            var marker = "CONTROL_DEFAULT=";
            var markerIndex = line.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (markerIndex < 0)
                continue;

            var profileId = line[(markerIndex + marker.Length)..].Trim();
            if (string.IsNullOrWhiteSpace(profileId))
                continue;

            if (!_controlProfiles.Any(p => p.Id.Equals(profileId, StringComparison.Ordinal)))
                continue;

            SetActiveControlProfile(profileId);
            _defaultControlProfileId = profileId;
            PersistDesktopOptions();
            AppendUiLog($"[SETTINGS] Применен default профиль из заметок: {profileId}");
            WriteSessionLog("CONTROL", $"defaultProfile={profileId}; source=operator-notes");
            return;
        }

        AppendUiLog("[SETTINGS] В заметках не найден CONTROL_DEFAULT=<profile-id>.");
    }

    private void TogglePauseMenu()
    {
        if (IsPauseMenuOpen)
        {
            ClosePauseMenu();
            return;
        }

        OpenPauseMenu();
    }

    private void OpenPauseMenu()
    {
        _pauseMenuOpen = true;
        _escMenuPage = EscMenuPage.Root;
        _activeOptionsPage = 0;
        _menuHitTargets.Clear();
        SetUiInputCapture(true, "pause-menu");
        _statusMessage = "Пауза: выберите действие в ESC-меню.";
        WriteSessionLog("MENU", "pauseMenu=open");
    }

    private void ClosePauseMenu()
    {
        _pauseMenuOpen = false;
        _menuHitTargets.Clear();
        SetUiInputCapture(false, "pause-menu");
        _statusMessage = "Пауза снята: управление восстановлено.";
        WriteSessionLog("MENU", "pauseMenu=closed");
    }

    private void HandlePauseMenuClick(Point location)
    {
        if (!IsPauseMenuOpen)
            return;

        foreach (var target in _menuHitTargets.AsEnumerable().Reverse())
        {
            if (!target.IsEnabled || !target.Bounds.Contains(location))
                continue;

            target.OnClick();
            return;
        }
    }

    private void DrawPauseMenuOverlay(Graphics g)
    {
        if (!IsPauseMenuOpen)
            return;

        _menuHitTargets.Clear();

        using var dim = new SolidBrush(Color.FromArgb(180, 6, 10, 18));
        g.FillRectangle(dim, 0, 0, _viewport.Width, _viewport.Height);

        var panel = new Rectangle(_viewport.Width / 2 - 290, _viewport.Height / 2 - 230, 580, 460);
        using var panelBg = new SolidBrush(Color.FromArgb(230, 16, 22, 34));
        using var panelBorder = new Pen(Color.FromArgb(220, 106, 180, 255), 2f);
        using var titleBrush = new SolidBrush(Color.FromArgb(245, 210, 108));
        using var infoBrush = new SolidBrush(Color.FromArgb(210, 224, 236));

        g.FillRectangle(panelBg, panel);
        g.DrawRectangle(panelBorder, panel);
        g.DrawString("ESC MENU", new Font(Font, FontStyle.Bold), titleBrush, panel.X + 20, panel.Y + 16);
        g.DrawString("Play / Save / Load / Options / Exit", Font, infoBrush, panel.X + 20, panel.Y + 44);

        if (_escMenuPage == EscMenuPage.Root)
            DrawPauseRootMenu(g, panel);
        else
            DrawPauseOptionsMenu(g, panel);

        if (_menuToastLines.Count > 0 && _clock.ElapsedMilliseconds <= _menuToastExpiresAtMs)
        {
            using var toastBg = new SolidBrush(Color.FromArgb(220, 18, 34, 52));
            using var toastBorder = new Pen(Color.FromArgb(220, 130, 200, 255), 1.5f);
            using var toastText = new SolidBrush(Color.FromArgb(235, 238, 245));
            var toastRect = new Rectangle(panel.X + 20, panel.Bottom - 74, panel.Width - 40, 44);
            g.FillRectangle(toastBg, toastRect);
            g.DrawRectangle(toastBorder, toastRect);
            g.DrawString(_menuToastLines[0], Font, toastText, toastRect.X + 10, toastRect.Y + 12);
        }
    }

    private void DrawPauseRootMenu(Graphics g, Rectangle panel)
    {
        var entries = new (string Label, Action OnClick)[]
        {
            ("Play", () => ClosePauseMenu()),
            ("Save", SaveGameFromMenu),
            ("Load", LoadGameFromMenu),
            ("Options", () => _escMenuPage = EscMenuPage.Options),
            ("Exit", ExitToDesktopFromMenu),
        };

        var y = panel.Y + 92;
        foreach (var entry in entries)
        {
            var rect = new Rectangle(panel.X + 40, y, panel.Width - 80, 52);
            DrawMenuButton(g, rect, entry.Label, true, entry.OnClick);
            y += 62;
        }
    }

    private void DrawPauseOptionsMenu(Graphics g, Rectangle panel)
    {
        var backRect = new Rectangle(panel.X + panel.Width - 140, panel.Y + 16, 104, 34);
        DrawMenuButton(g, backRect, "Back", true, () => _escMenuPage = EscMenuPage.Root, compact: true);

        var tabY = panel.Y + 74;
        var categories = Enum.GetValues<OptionsCategory>();
        var tabWidth = (panel.Width - 42) / categories.Length;
        for (var i = 0; i < categories.Length; i++)
        {
            var category = categories[i];
            var rect = new Rectangle(panel.X + 20 + (i * tabWidth), tabY, tabWidth - 6, 30);
            var active = category == _activeOptionsCategory;
            DrawMenuButton(g, rect, category.ToString(), true, () =>
            {
                _activeOptionsCategory = category;
                _activeOptionsPage = 0;
            }, compact: true, accent: active);
        }

        var optionRows = BuildOptionRows(_activeOptionsCategory);
        var rowY = tabY + 44;
        var rowsPerPage = Math.Max(1, (panel.Bottom - 122 - rowY) / 36);
        var totalPages = Math.Max(1, (int)Math.Ceiling(optionRows.Count / (double)rowsPerPage));
        _activeOptionsPage = Math.Clamp(_activeOptionsPage, 0, totalPages - 1);
        var pagedRows = optionRows
            .Skip(_activeOptionsPage * rowsPerPage)
            .Take(rowsPerPage);

        foreach (var row in pagedRows)
        {
            var labelRect = new Rectangle(panel.X + 26, rowY, 280, 30);
            var valueRect = new Rectangle(panel.X + 314, rowY, panel.Width - 340, 30);

            using var labelBrush = new SolidBrush(Color.FromArgb(220, 230, 240));
            g.DrawString(row.Label, Font, labelBrush, labelRect.X, labelRect.Y + 6);
            DrawMenuButton(g, valueRect, row.Value, row.IsEnabled, row.OnClick, compact: true, accent: row.IsEnabled);

            rowY += 36;
        }

        if (totalPages > 1)
        {
            var prevRect = new Rectangle(panel.X + 26, panel.Bottom - 72, 100, 32);
            var nextRect = new Rectangle(panel.Right - 126, panel.Bottom - 72, 100, 32);
            var pageTextRect = new Rectangle(panel.X + 136, panel.Bottom - 72, panel.Width - 272, 32);
            DrawMenuButton(g, prevRect, "Prev", _activeOptionsPage > 0, () => _activeOptionsPage--, compact: true);
            DrawMenuButton(g, nextRect, "Next", _activeOptionsPage < totalPages - 1, () => _activeOptionsPage++, compact: true);
            using var pageBrush = new SolidBrush(Color.FromArgb(200, 226, 236));
            g.DrawString($"Page {_activeOptionsPage + 1}/{totalPages}", Font, pageBrush, pageTextRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }
    }

    private List<(string Label, string Value, bool IsEnabled, Action OnClick)> BuildOptionRows(OptionsCategory category)
    {
        var rows = new List<(string Label, string Value, bool IsEnabled, Action OnClick)>();

        float NextFromSteps(float current, params float[] steps)
        {
            for (var i = 0; i < steps.Length; i++)
            {
                if (MathF.Abs(current - steps[i]) <= 0.0005f)
                    return steps[(i + 1) % steps.Length];
            }

            return steps[0];
        }

        MouseControlButton NextMouseButton(MouseControlButton current)
        {
            var values = Enum.GetValues<MouseControlButton>();
            var currentIndex = Array.IndexOf(values, current);
            if (currentIndex < 0)
                currentIndex = 0;
            return values[(currentIndex + 1) % values.Length];
        }

        string BindingLabel(Keys key) => key == Keys.None ? "OFF" : key.ToString();

        Keys NextBindingKey(Keys current)
        {
            var keys = new[]
            {
                Keys.None,
                Keys.W, Keys.A, Keys.S, Keys.D,
                Keys.Up, Keys.Left, Keys.Down, Keys.Right,
                Keys.I, Keys.J, Keys.K, Keys.L,
                Keys.E, Keys.F, Keys.Q, Keys.R,
                Keys.Space, Keys.LeftShift, Keys.RightShift,
                Keys.ControlKey, Keys.Tab,
            };

            var currentIndex = Array.IndexOf(keys, current);
            if (currentIndex < 0)
                currentIndex = 0;

            return keys[(currentIndex + 1) % keys.Length];
        }

        switch (category)
        {
            case OptionsCategory.Controls:
                DesktopControlProfileEntry? activeProfile = _controlProfiles
                    .FirstOrDefault(p => p.Id.Equals(_activeControlProfileId, StringComparison.Ordinal));

                void UpdateProfile(Func<ControlV2Profile, ControlV2Profile> updater)
                {
                    _controlProfile = ControlProfilePresetCatalog.Normalize(updater(_controlProfile));
                    SaveActiveControlProfileToCollection();
                    PersistDesktopOptions();
                }

                rows.Add((
                    "Active Profile",
                    activeProfile?.DisplayName ?? _activeControlProfileId,
                    _controlProfiles.Count > 1,
                    () =>
                    {
                        if (_controlProfiles.Count <= 1)
                            return;

                        var currentIndex = _controlProfiles.FindIndex(p => p.Id.Equals(_activeControlProfileId, StringComparison.Ordinal));
                        if (currentIndex < 0)
                            currentIndex = 0;

                        var nextIndex = (currentIndex + 1) % _controlProfiles.Count;
                        SetActiveControlProfile(_controlProfiles[nextIndex].Id);
                        PersistDesktopOptions();
                    }));

                rows.Add((
                    "Scheme",
                    _controlProfile.SchemeId.ToString(),
                    true,
                    () =>
                    {
                        var schemes = Enum.GetValues<ControlSchemeId>();
                        var currentIndex = Array.IndexOf(schemes, _controlProfile.SchemeId);
                        var nextScheme = schemes[(currentIndex + 1) % schemes.Length];
                        UpdateProfile(p => p with { SchemeId = nextScheme });
                    }));

                rows.Add((
                    "Create Custom",
                    "RUN",
                    true,
                    CreateCustomProfileFromActive));

                rows.Add((
                    "Delete Active Custom",
                    "RUN",
                    activeProfile?.IsCustom == true,
                    DeleteActiveCustomProfile));

                rows.Add((
                    "Reset Active Profile",
                    "RUN",
                    true,
                    ResetActiveProfileToPresetDefaults));

                rows.Add((
                    "Forward Speed",
                    $"{_controlProfile.MoveSpeedForwardMultiplier:F2}x",
                    true,
                    () =>
                    {
                        UpdateProfile(p => p with
                        {
                            MoveSpeedForwardMultiplier = NextFromSteps(p.MoveSpeedForwardMultiplier, 0.75f, 0.90f, 1.00f, 1.15f, 1.30f),
                        });
                    }));

                rows.Add((
                    "Backward Speed",
                    $"{_controlProfile.MoveSpeedBackwardMultiplier:F2}x",
                    true,
                    () =>
                    {
                        UpdateProfile(p => p with
                        {
                            MoveSpeedBackwardMultiplier = NextFromSteps(p.MoveSpeedBackwardMultiplier, 0.50f, 0.70f, 0.82f, 1.00f),
                        });
                    }));

                rows.Add((
                    "Strafe Speed",
                    $"{_controlProfile.MoveSpeedStrafeMultiplier:F2}x",
                    true,
                    () =>
                    {
                        UpdateProfile(p => p with
                        {
                            MoveSpeedStrafeMultiplier = NextFromSteps(p.MoveSpeedStrafeMultiplier, 0.50f, 0.80f, 1.00f, 1.20f),
                        });
                    }));

                rows.Add((
                    "Turn Speed",
                    $"{_controlProfile.TurnSpeed:F2}",
                    true,
                    () =>
                    {
                        UpdateProfile(p => p with
                        {
                            TurnSpeed = NextFromSteps(p.TurnSpeed, 1.4f, 2.0f, 2.8f, 3.6f, 4.4f),
                        });
                    }));

                rows.Add((
                    "Rotation Smoothing",
                    $"{_controlProfile.RotationSmoothing:F1}",
                    true,
                    () =>
                    {
                        UpdateProfile(p => p with
                        {
                            RotationSmoothing = NextFromSteps(p.RotationSmoothing, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f),
                        });
                    }));

                rows.Add((
                    "Invert Turn Input",
                    _controlProfile.InvertTurnInput ? "ON" : "OFF",
                    true,
                    () => UpdateProfile(p => p with { InvertTurnInput = !p.InvertTurnInput })));

                rows.Add((
                    "Input Deadzone",
                    $"{_controlProfile.Deadzone:F3}",
                    true,
                    () =>
                    {
                        UpdateProfile(p => p with
                        {
                            Deadzone = NextFromSteps(p.Deadzone, 0.000f, 0.010f, 0.020f, 0.030f, 0.040f),
                        });
                    }));
                break;

            case OptionsCategory.Camera:
                rows.Add((
                    "Orbit Sensitivity H",
                    $"{_controlProfile.OrbitSensitivity:F4}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            OrbitSensitivity = NextFromSteps(_controlProfile.OrbitSensitivity, 0.0040f, 0.0060f, 0.0075f, 0.0100f, 0.0130f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Orbit Sensitivity V",
                    $"{_controlProfile.OrbitSensitivityVertical:F4}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            OrbitSensitivityVertical = NextFromSteps(_controlProfile.OrbitSensitivityVertical, 0.0020f, 0.0030f, 0.0040f, 0.0050f, 0.0060f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Camera Smoothing",
                    $"{_controlProfile.CameraSmoothing:F1}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            CameraSmoothing = NextFromSteps(_controlProfile.CameraSmoothing, 3.5f, 5.5f, 7.5f, 9.5f, 12.0f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Camera Distance",
                    $"{_controlProfile.CameraDistance:F1}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            CameraDistance = NextFromSteps(_controlProfile.CameraDistance, 4.5f, 5.5f, 6.0f, 7.0f, 8.0f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Camera Height",
                    $"{_controlProfile.CameraHeight:F1}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            CameraHeight = NextFromSteps(_controlProfile.CameraHeight, 3.6f, 4.2f, 4.8f, 5.4f, 6.0f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Max Orbit Yaw",
                    $"{_controlProfile.MaxOrbitOffsetYaw:F2}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            MaxOrbitOffsetYaw = NextFromSteps(_controlProfile.MaxOrbitOffsetYaw, 0.55f, 0.85f, 1.05f, 1.25f, 1.45f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Max Orbit Vertical",
                    $"{_controlProfile.MaxOrbitVerticalOffset:F2}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            MaxOrbitVerticalOffset = NextFromSteps(_controlProfile.MaxOrbitVerticalOffset, 0.60f, 1.00f, 1.40f, 1.80f, 2.40f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Invert Horizontal Orbit",
                    _controlProfile.InvertOrbitHorizontal ? "ON" : "OFF",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with { InvertOrbitHorizontal = !_controlProfile.InvertOrbitHorizontal };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Invert Vertical Orbit",
                    _controlProfile.InvertOrbitVertical ? "ON" : "OFF",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with { InvertOrbitVertical = !_controlProfile.InvertOrbitVertical };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Orbit Recenter",
                    _controlProfile.OrbitRecenteringEnabled ? "ON" : "OFF",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with { OrbitRecenteringEnabled = !_controlProfile.OrbitRecenteringEnabled };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Recenter Speed Move",
                    $"{_controlProfile.OrbitRecenteringMovingSpeed:F1}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            OrbitRecenteringMovingSpeed = NextFromSteps(_controlProfile.OrbitRecenteringMovingSpeed, 3.0f, 6.0f, 9.5f, 12.0f, 16.0f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                rows.Add((
                    "Recenter Speed Idle",
                    $"{_controlProfile.OrbitRecenteringIdleSpeed:F1}",
                    true,
                    () =>
                    {
                        _controlProfile = _controlProfile with
                        {
                            OrbitRecenteringIdleSpeed = NextFromSteps(_controlProfile.OrbitRecenteringIdleSpeed, 1.0f, 3.0f, 5.5f, 8.0f, 11.0f),
                        };
                        SaveActiveControlProfileToCollection();
                        PersistDesktopOptions();
                    }));
                break;

            case OptionsCategory.Graphics:
                rows.Add(("Capsule Steam FX", _fxCapsuleSteamEnabled ? "ON" : "OFF", true, () =>
                {
                    _fxCapsuleSteamEnabled = !_fxCapsuleSteamEnabled;
                    PersistDesktopOptions();
                }));
                rows.Add(("CORE Hologram FX", _fxCoreHologramEnabled ? "ON" : "OFF", true, () =>
                {
                    _fxCoreHologramEnabled = !_fxCoreHologramEnabled;
                    PersistDesktopOptions();
                }));
                rows.Add(("Archive Route FX", _fxArchiveRouteGuideEnabled ? "ON" : "OFF", true, () =>
                {
                    _fxArchiveRouteGuideEnabled = !_fxArchiveRouteGuideEnabled;
                    PersistDesktopOptions();
                }));
                rows.Add(("Shadow Quality", "PLANNED", false, () => { }));
                break;

            case OptionsCategory.Audio:
                rows.Add(("Master Volume", "PLANNED", false, () => { }));
                rows.Add(("Dialogue Volume", "PLANNED", false, () => { }));
                rows.Add(("FX Volume", "PLANNED", false, () => { }));
                break;

            case OptionsCategory.Ui:
                rows.Add(("Detailed HUD", _showExtendedHudTelemetry ? "ON" : "OFF", true, () =>
                {
                    _showExtendedHudTelemetry = !_showExtendedHudTelemetry;
                    PersistDesktopOptions();
                }));
                rows.Add(("HUD Scale", "PLANNED", false, () => { }));
                rows.Add(("Subtitles", "PLANNED", false, () => { }));
                break;

            case OptionsCategory.Gameplay:
                rows.Add(("Pause Stops Gameplay", "ON", false, () => { }));
                rows.Add(("Strict UI Input Lock", StrictUiInputLockEnabled ? "ON" : "OFF", false, () => { }));
                rows.Add(("Tank Movement", _controlProfile.UseTankMovement ? "ON" : "OFF", true, () =>
                {
                    _controlProfile = _controlProfile with { UseTankMovement = !_controlProfile.UseTankMovement };
                    SaveActiveControlProfileToCollection();
                    PersistDesktopOptions();
                }));
                rows.Add(("Tank Turning", _controlProfile.UseTankTurning ? "ON" : "OFF", true, () =>
                {
                    _controlProfile = _controlProfile with { UseTankTurning = !_controlProfile.UseTankTurning };
                    SaveActiveControlProfileToCollection();
                    PersistDesktopOptions();
                }));
                rows.Add(("Face Move Direction", _controlProfile.FaceMovementDirection ? "ON" : "OFF", true, () =>
                {
                    _controlProfile = _controlProfile with { FaceMovementDirection = !_controlProfile.FaceMovementDirection };
                    SaveActiveControlProfileToCollection();
                    PersistDesktopOptions();
                }));
                rows.Add(("Face Camera On Move", _controlProfile.FaceCameraDirectionWhenMoving ? "ON" : "OFF", true, () =>
                {
                    _controlProfile = _controlProfile with { FaceCameraDirectionWhenMoving = !_controlProfile.FaceCameraDirectionWhenMoving };
                    SaveActiveControlProfileToCollection();
                    PersistDesktopOptions();
                }));
                rows.Add(("Face Camera Idle", _controlProfile.FaceCameraDirectionWhenIdle ? "ON" : "OFF", true, () =>
                {
                    _controlProfile = _controlProfile with { FaceCameraDirectionWhenIdle = !_controlProfile.FaceCameraDirectionWhenIdle };
                    SaveActiveControlProfileToCollection();
                    PersistDesktopOptions();
                }));
                rows.Add(("Normalize Diagonal", _controlProfile.NormalizeDiagonalMovement ? "ON" : "OFF", true, () =>
                {
                    _controlProfile = _controlProfile with { NormalizeDiagonalMovement = !_controlProfile.NormalizeDiagonalMovement };
                    SaveActiveControlProfileToCollection();
                    PersistDesktopOptions();
                }));
                rows.Add(("Decouple Camera Yaw", _controlProfile.DecoupleCameraYaw ? "ON" : "OFF", true, () =>
                {
                    _controlProfile = _controlProfile with { DecoupleCameraYaw = !_controlProfile.DecoupleCameraYaw };
                    SaveActiveControlProfileToCollection();
                    PersistDesktopOptions();
                }));
                rows.Add(("Auto-save", "PLANNED", false, () => { }));
                break;

            case OptionsCategory.Admin:
                rows.Add(("Unlock Rooms", _adminUnlockAllRooms ? "ON" : "OFF", true, () =>
                {
                    _adminUnlockAllRooms = !_adminUnlockAllRooms;
                    PersistDesktopOptions();
                }));
                rows.Add(("Unlock Terminals", _adminUnlockAllTerminals ? "ON" : "OFF", true, () =>
                {
                    _adminUnlockAllTerminals = !_adminUnlockAllTerminals;
                    PersistDesktopOptions();
                }));
                rows.Add(("Hard Archive Preview", _adminUnlockHardArchivePreview ? "ON" : "OFF", true, () =>
                {
                    _adminUnlockHardArchivePreview = !_adminUnlockHardArchivePreview;
                    if (_adminUnlockHardArchivePreview && _archivePreviewRooms.Count == 0)
                        _archivePreviewRooms = Session10MissionCatalog.InitialArchivePreviewRooms;
                    if (!_adminUnlockHardArchivePreview)
                        _archivePreviewRooms = [];
                    PersistDesktopOptions();
                }));
                rows.Add(("Purchase Simulation", _adminEnablePurchaseSimulation ? "ON" : "OFF", true, () =>
                {
                    _adminEnablePurchaseSimulation = !_adminEnablePurchaseSimulation;
                    PersistDesktopOptions();
                }));
                rows.Add(("Mission Debug Page", _adminEnableMissionPageDebugAccess ? "ON" : "OFF", true, () =>
                {
                    _adminEnableMissionPageDebugAccess = !_adminEnableMissionPageDebugAccess;
                    PersistDesktopOptions();
                }));
                rows.Add(("Force OperationAccess", "RUN", true, () => ForceOperationAccessPhase()));
                rows.Add(("Reset Admin Defaults", "RUN", true, () =>
                {
                    EnsureAdminFlagsInitialized();
                    PersistDesktopOptions();
                }));
                break;
        }

        return rows;
    }

    private void DrawMenuButton(Graphics g, Rectangle rect, string text, bool enabled, Action onClick, bool compact = false, bool accent = false)
    {
        var bgColor = !enabled
            ? Color.FromArgb(120, 64, 68, 76)
            : accent
                ? Color.FromArgb(210, 46, 88, 132)
                : Color.FromArgb(200, 34, 46, 62);
        var borderColor = enabled
            ? Color.FromArgb(230, 112, 180, 245)
            : Color.FromArgb(160, 92, 96, 110);
        var textColor = enabled
            ? Color.FromArgb(236, 240, 246)
            : Color.FromArgb(168, 174, 182);

        using var bg = new SolidBrush(bgColor);
        using var border = new Pen(borderColor, compact ? 1.2f : 1.8f);
        using var textBrush = new SolidBrush(textColor);

        g.FillRectangle(bg, rect);
        g.DrawRectangle(border, rect);

        var textSize = g.MeasureString(text, Font);
        var tx = rect.X + (rect.Width - textSize.Width) * 0.5f;
        var ty = rect.Y + (rect.Height - textSize.Height) * 0.5f;
        g.DrawString(text, Font, textBrush, tx, ty);

        _menuHitTargets.Add(new MenuHitTarget(rect, onClick, enabled));
    }

    private void SaveGameFromMenu()
    {
        if (_bundle is null || string.IsNullOrWhiteSpace(_saveGamePath))
            return;

        var baseSave = File.Exists(_saveGamePath)
            ? _saveGameStore.LoadOrCreate(_saveGamePath, DesktopSaveVersion)
            : new SaveGame { Version = DesktopSaveVersion };

        var snapshot = SaveGameMapper.Extract(
            _bundle.HubRuntime,
            _bundle.PrologueTracker,
            _bundle.ObjectiveTracker,
            _bundle.Inventory,
            operatorXp: 145,
            operatorLevel: 1,
            baseSave: baseSave);

        _saveGameStore.Save(_saveGamePath, snapshot);
        _statusMessage = "Игра сохранена.";
        WriteSessionLog("SAVE", $"saved=true; file={_saveGamePath}");
        _menuToastLines.Clear();
        _menuToastLines.Add("Save completed.");
        _menuToastExpiresAtMs = _clock.ElapsedMilliseconds + 2200;
    }

    private void LoadGameFromMenu()
    {
        if (string.IsNullOrWhiteSpace(_saveGamePath) || !File.Exists(_saveGamePath))
        {
            _menuToastLines.Clear();
            _menuToastLines.Add("Save file not found.");
            _menuToastExpiresAtMs = _clock.ElapsedMilliseconds + 2200;
            return;
        }

        var save = _saveGameStore.LoadOrCreate(_saveGamePath, DesktopSaveVersion);

        InitializeRuntime();
        if (_bundle is null)
            return;

        SaveGameMapper.Apply(
            save,
            _bundle.HubRuntime,
            _bundle.PrologueTracker,
            _bundle.ObjectiveTracker,
            _bundle.Inventory);

        _statusMessage = "Сохранение загружено.";
        WriteSessionLog("SAVE", $"loaded=true; file={_saveGamePath}");
        _menuToastLines.Clear();
        _menuToastLines.Add("Load completed.");
        _menuToastExpiresAtMs = _clock.ElapsedMilliseconds + 2200;
    }

    private void ExitToDesktopFromMenu()
    {
        WriteSessionLog("MENU", "exit=desktop-requested");
        Close();
    }

    private void ForceOperationAccessPhase()
    {
        if (_bundle is null)
            return;

        var sequence = new[] { "capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console" };
        foreach (var objectId in sequence)
            _bundle.HubRuntime.Interact(objectId);

        _statusMessage = "ADMIN: Фаза принудительно синхронизирована до OperationAccess.";
        AppendUiLog("[ADMIN] Фаза переведена в OperationAccess.");
        WriteSessionLog("ADMIN", "phase-forced=operation-access");
    }

    private bool TryCanonicalInteractionFallback()
    {
        if (_bundle is null || string.IsNullOrWhiteSpace(_focusedObjectId))
            return false;

        if (_focusedObjectId is not ("capsule_exit" or "bio_scanner" or "drone_dock" or "core_console"))
            return false;

        var target = _bundle.HubRuntime.Zones
            .SelectMany(z => z.Objects)
            .FirstOrDefault(o => o.Id.Equals(_focusedObjectId, StringComparison.Ordinal));

        if (target is null)
            return false;

        var distance = (target.Position - _bundle.Player.Position).Length();
        if (distance > 2.6f)
            return false;

        var fallbackResult = _bundle.HubRuntime.Interact(target.Id);
        if (!fallbackResult.Success)
            return false;

        _bundle.PrologueTracker.RecordVisit(target.Id);
        _statusMessage = fallbackResult.Message;
        AppendUiLog($"[INTERACT] {fallbackResult.ObjectId} -> OK | {fallbackResult.Message} (fallback)");
        WriteSessionLog("INTERACT", $"object={fallbackResult.ObjectId}; success=true; phase={_bundle.HubRuntime.CurrentPhase}; source=fallback");
        return true;
    }

    private static string ResolveContentRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        for (var i = 0; i < 10 && dir is not null; i++)
        {
            var candidate = Path.Combine(dir.FullName, "Content");
            var zones = Path.Combine(candidate, "Zones", "A0_Zones.json");
            if (File.Exists(zones))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Unable to find Content root (expected Content/Zones/A0_Zones.json). ");
    }
}
