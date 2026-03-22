using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Input;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Domain.World.Runtime;
using BabylonArchiveCore.Infrastructure.Logging;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.Mission;
using System.Diagnostics;
using System.Text;

namespace BabylonArchiveCore.Desktop;

public sealed class PrologueClientForm : Form
{
    private sealed record ExtraInteractionNode(string Id, string DisplayName, Vec3 Position, float Radius);
    private sealed record EnvModule(string Id, Vec3 Position, Vec3 Size, Color Color, bool Emissive = false);

    private static readonly ExtraInteractionNode[] ExtraNodes =
    [
        new("commerce_hall", "Коммерческий зал", new Vec3(-6.0f, 0.0f, 6.0f), 2.2f),
        new("tech_hall", "Технический зал", new Vec3(0.0f, 0.0f, -8.0f), 2.2f),
        new("archive_preview", "Контур Хард-Архива", new Vec3(0.0f, 0.0f, -15.0f), 2.5f),
        new("mission_board", "Доска операций", new Vec3(0.0f, 0.0f, 8.0f), 2.0f),
        new("research_lab", "Аналитический узел", new Vec3(6.0f, 0.0f, 6.0f), 2.0f),
        new("archive_control", "Контур контроля архива", new Vec3(0.0f, 0.0f, 11.5f), 2.0f),
        new("tool_bench", "Сборочный стол", new Vec3(2.5f, 0.0f, -9.5f), 1.8f),
        new("commerce_desk", "Сервисный модуль", new Vec3(-8.6f, 0.0f, 5.3f), 1.8f),
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
    ];

    private readonly WorldRuntimeProfile _runtimeProfile;
    private readonly Panel _viewport = new() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(10, 12, 18) };
    private readonly RichTextBox _logBox = new() { ReadOnly = true, Dock = DockStyle.Fill, Font = new Font("Consolas", 9f) };
    private readonly TextBox _noteInput = new() { Dock = DockStyle.Fill, Font = new Font("Consolas", 9f), PlaceholderText = "Введите заметку для Copilot и нажмите Enter или кнопку Сохранить" };
    private readonly Button _saveNoteButton = new() { Text = "Сохранить", Dock = DockStyle.Fill, Height = 30 };
    private readonly System.Windows.Forms.Timer _loopTimer = new() { Interval = 16 };
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private long _lastTicks;

    private readonly HashSet<Keys> _keysDown = [];
    private readonly HashSet<Keys> _keysPressed = [];
    private bool _rightMouseDown;
    private Point _lastMousePos;

    private float _freeYaw;
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
    private string? _notesLogPath;
    private HubRhythmPhase _lastLoggedPhase = HubRhythmPhase.Awakening;
    private string? _lastLoggedObjectiveId;
    private string? _lastLoggedFocus;
    private string? _lastLoggedTerminalId;
    private CameraMode _lastLoggedCameraMode = CameraMode.Full3D;
    private bool _lastLoggedProtocolZero;
    private string? _lastLoggedStatusMessage;
    private long _lastHeartbeatMs;

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
        Text = "Babylon Archive Core - Session 9 Gameplay Harness";
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
            if (e.Button == MouseButtons.Right)
            {
                _rightMouseDown = true;
                _lastMousePos = e.Location;
            }
        };
        _viewport.MouseUp += (_, e) =>
        {
            if (e.Button == MouseButtons.Right)
                _rightMouseDown = false;
        };
        _viewport.MouseMove += (_, e) =>
        {
            if (_rightMouseDown)
            {
                var deltaX = e.X - _lastMousePos.X;
                _freeYaw += deltaX * 0.0075f;
                _lastMousePos = e.Location;
            }
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
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SaveOperatorNote();
            }
        };

        KeyDown += (_, e) =>
        {
            if (_keysDown.Add(e.KeyCode))
                _keysPressed.Add(e.KeyCode);
        };

        KeyUp += (_, e) => _keysDown.Remove(e.KeyCode);
    }

    private void InitializeRuntime()
    {
        var contentRoot = ResolveContentRoot();
        var runtimeDir = Path.Combine(AppContext.BaseDirectory, "runtime");
        Directory.CreateDirectory(runtimeDir);
        _sessionLogPath = Path.Combine(runtimeDir, "desktop-session.log");
        _notesLogPath = Path.Combine(runtimeDir, "operator-notes.log");
        var logger = new FileLogger(Path.Combine(runtimeDir, "desktop-client.log"));

        var provider = new A0ContentProvider(contentRoot);
        var factory = new ContentDrivenSceneFactory(provider, logger);
        _bundle = factory.Build();

        _hints = new HintSystem();
        _hints.RegisterFromZones(_bundle.HubRuntime.Zones);

        _objectiveHud = new ObjectiveHUD(_bundle.ObjectiveTracker);
        _objectiveHud.Update();

        _missionSlots = Session10MissionCatalog.BuildInitialBoard(_runtimeProfile.AccessMode == WorldAccessMode.Admin);
        _archivePreviewRooms = _runtimeProfile.UnlockHardArchivePreview
            ? Session10MissionCatalog.InitialArchivePreviewRooms
            : [];

        _logBox.Clear();
        AppendUiLog("[СИСТЕМА] Клиент пролога запущен.");
        AppendUiLog($"[СИСТЕМА] Контент: {contentRoot}");
        AppendUiLog($"[СИСТЕМА] Режим доступа: {_runtimeProfile.AccessMode}");
        AppendUiLog($"[СИСТЕМА] Unlock-флаги: rooms={_runtimeProfile.UnlockAllRooms}, terminals={_runtimeProfile.UnlockAllTerminals}, archive={_runtimeProfile.UnlockHardArchivePreview}");
        AppendUiLog("[УПРАВЛЕНИЕ] WASD движение | E взаимодействие | Q камера | SPACE диалоги | ESC закрыть терминал | R перезапуск | ПКМ+мышь обзор");
        AppendUiLog("[ТЕСТ] Шаг 1: нажмите E у capsule_exit -> bio_scanner");
        AppendUiLog("[ТЕСТ] Шаг 2: нажмите E у bio_scanner -> supply_terminal");
        AppendUiLog("[ТЕСТ] Шаг 3: нажмите E у supply_terminal -> drone_dock -> core_console");
        AppendUiLog("[ТЕСТ] Шаг 4: после OperationAccess проверьте op_terminal + research_terminal + gallery_overlook + archive_gate");
        AppendUiLog("[ТЕСТ] Шаг 5: проверьте Q/SPACE/ESC/R и фиксируйте результат в логе");
        if (_runtimeProfile.AccessMode == WorldAccessMode.Admin)
            AppendUiLog("[ТЕСТ] Шаг 6 (ADMIN): mission_board / tech_hall / commerce_hall / archive_preview / tool_bench / archive_control");
        AppendUiLog("[ЗАМЕТКИ] Введите текст ниже и нажмите Enter/Сохранить. Заметка попадет в runtime/operator-notes.log.");

        _statusMessage = _runtimeProfile.AccessMode == WorldAccessMode.Admin
            ? "Режим ADMIN активен. Превью-комнаты разблокированы."
            : "Оператор, пробуждение завершено.";
        _activeTerminal = null;
        _focusedObjectId = null;
        _focusedObjectName = null;
        _focusedExtraId = null;
        _focusedExtraName = null;
        _freeYaw = 0f;
        _protocolObjectiveApplied = false;

        _lastLoggedPhase = _bundle.HubRuntime.CurrentPhase;
        _lastLoggedObjectiveId = _bundle.ObjectiveTracker.GetActive()?.ObjectiveId;
        _lastLoggedFocus = null;
        _lastLoggedTerminalId = null;
        _lastLoggedCameraMode = _bundle.Session.Camera.ActiveMode;
        _lastLoggedProtocolZero = _bundle.PrologueTracker.IsProtocolZeroUnlocked;
        _lastLoggedStatusMessage = _statusMessage;
        _lastHeartbeatMs = 0;

        WriteSessionLog("SESSION", "========== NEW DESKTOP SESSION ==========");
        WriteSessionLog("SYSTEM", $"Boot time={DateTimeOffset.Now:O}");
        WriteSessionLog("SYSTEM", $"Mode={_runtimeProfile.AccessMode}; unlockRooms={_runtimeProfile.UnlockAllRooms}; unlockTerminals={_runtimeProfile.UnlockAllTerminals}; archivePreview={_runtimeProfile.UnlockHardArchivePreview}");
        WriteSessionLog("SYSTEM", "Controls: WASD, E interact, Q camera, SPACE next dialogue, ESC close terminal, R reset, RMB drag free look");
        WriteSessionLog("TEST", "1) Capsule -> 2) Biometrics -> 3) Logistics -> 4) Drone -> 5) CORE -> 6) Operations/Research/Gallery/Gate");
        WriteSessionLog("NOTES", "Operator notes file: runtime/operator-notes.log");
    }

    private void TickFrame()
    {
        if (_bundle is null || _hints is null || _objectiveHud is null)
            return;

        var nowTicks = _clock.ElapsedTicks;
        var dt = (float)(nowTicks - _lastTicks) / Stopwatch.Frequency;
        _lastTicks = nowTicks;
        dt = Math.Clamp(dt, 0.001f, 0.05f);

        var advanceDialogue = ConsumePressed(Keys.Space);
        var closeOverlay = ConsumePressed(Keys.Escape);
        var reset = ConsumePressed(Keys.R);

        if (advanceDialogue)
            WriteSessionLog("INPUT", "SPACE pressed (dialogue advance)");
        if (closeOverlay)
            WriteSessionLog("INPUT", "ESC pressed (close overlay)");
        if (reset)
            WriteSessionLog("INPUT", "R pressed (session reset)");

        if (reset)
        {
            InitializeRuntime();
            return;
        }

        if (closeOverlay)
        {
            _activeTerminal = null;
            AppendUiLog("[РЕЗУЛЬТАТ] Терминал закрыт.");
            WriteSessionLog("RESULT", "Overlay closed by ESC");
        }

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

        var blockedByOverlay = _activeTerminal is not null;
        var interactPressed = !blockedByOverlay && ConsumePressed(Keys.E);

        var input = InputSnapshot.FromActions(
            up: !blockedByOverlay && _keysDown.Contains(Keys.W),
            down: !blockedByOverlay && _keysDown.Contains(Keys.S),
            left: !blockedByOverlay && _keysDown.Contains(Keys.A),
            right: !blockedByOverlay && _keysDown.Contains(Keys.D),
            interact: interactPressed,
            cameraToggle: ConsumePressed(Keys.Q));

        if (input.CameraTogglePressed)
            WriteSessionLog("INPUT", "Q pressed (camera toggle)");
        if (interactPressed)
            WriteSessionLog("INPUT", "E pressed (interaction attempt)");

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

        RecordBackgroundTelemetry();

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

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.FromArgb(12, 15, 24));

        DrawFloorGrid(g);
        DrawEnvironmentShell(g);
        DrawStaticWhitebox(g);
        DrawZonesAndObjects(g);
        DrawExtraNodes(g);
        DrawPlayer(g);
        DrawHud(g);
        DrawDialogueOverlay(g);
        DrawTerminalOverlay(g);
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

        if (_runtimeProfile.UnlockHardArchivePreview)
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

        if (_runtimeProfile.EnablePurchaseSimulation)
        {
            DrawMarkerCube(g, new Vec3(-9.5f, 1.2f, -7.8f), new Vec3(1.2f, 2.0f, 1.2f), Color.FromArgb(230, 210, 110), 2.5f);
            DrawMarkerCube(g, new Vec3(9.5f, 1.2f, -7.8f), new Vec3(1.2f, 2.0f, 1.2f), Color.FromArgb(110, 210, 230), 2.5f);
        }
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

            var baseSize = obj.InteractiveType switch
            {
                InteractiveType.Terminal => new Vec3(1.0f, 1.9f, 1.0f),
                InteractiveType.Npc => new Vec3(0.75f, 1.35f, 0.75f),
                InteractiveType.Gate => new Vec3(1.8f, 2.6f, 1.2f),
                _ => new Vec3(0.8f, 1.6f, 0.8f),
            };

            var offsetY = obj.InteractiveType == InteractiveType.Npc
                ? 1.2f + 0.12f * MathF.Sin(t * 2.2f)
                : 1.2f;

            var color = visited
                ? Color.FromArgb(80, 200, 90)
                : isActive
                    ? Color.FromArgb(95, 170, 255)
                    : Color.FromArgb(170, 75, 75);

            if (isFocused)
                color = Color.FromArgb(255, 225, 95);

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

            var size = node.Id switch
            {
                "archive_preview" => new Vec3(1.2f, 1.8f, 1.2f),
                "mission_board" => new Vec3(1.6f, 2.1f, 1.0f),
                "archive_control" => new Vec3(1.2f, 2.4f, 1.2f),
                _ => new Vec3(0.9f, 1.7f, 0.9f),
            };

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

        g.FillRectangle(hudBg, 14, 14, 520, 160);
        g.DrawString($"Scene: SCN_A0_INITIATION", Font, textBrush, 24, 24);
        g.DrawString($"Phase: {_bundle.HubRuntime.CurrentPhase}", Font, textBrush, 24, 46);
        g.DrawString($"Zone: {_bundle.Session.PlayerCtrl.GetCurrentZone()?.Id}", Font, textBrush, 24, 68);
        g.DrawString($"Objective: {_objectiveHud.DisplayText}", Font, accentBrush, 24, 90);
        g.DrawString($"Protocol Zero: {_bundle.PrologueTracker.IsProtocolZeroUnlocked}", Font, textBrush, 24, 112);
        g.DrawString($"Camera: {_bundle.Session.Camera.ActiveMode} | FPS: {_fps:F0}", Font, textBrush, 24, 134);
        g.DrawString($"Mode: {_runtimeProfile.AccessMode}", Font, accentBrush, 350, 134);

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

        var cam = _bundle.Session.Camera.Position;
        var look = _bundle.Player.Position;

        var fwd = (look - cam).Normalized();
        fwd = RotateY(fwd, _freeYaw).Normalized();

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
            if (_focusedObjectId == "archive_gate" && _runtimeProfile.UnlockHardArchivePreview)
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
            "commerce_hall" => isOperationAccess || _runtimeProfile.UnlockAllRooms,
            "commerce_desk" => isOperationAccess || _runtimeProfile.EnablePurchaseSimulation || _runtimeProfile.UnlockAllRooms,
            "tech_hall" => isOperationAccess || _runtimeProfile.UnlockAllRooms,
            "tool_bench" => isOperationAccess || _runtimeProfile.UnlockAllRooms,
            "archive_preview" => _runtimeProfile.UnlockHardArchivePreview,
            "mission_board" => isOperationAccess || _runtimeProfile.UnlockAllTerminals,
            "research_lab" => isOperationAccess || _runtimeProfile.UnlockAllTerminals,
            "archive_control" => isOperationAccess || _runtimeProfile.UnlockHardArchivePreview,
            _ => false,
        };
    }

    private void HandleExtraInteraction(string nodeId)
    {
        WriteSessionLog("INTERACT", $"extraNode={nodeId}");
        switch (nodeId)
        {
            case "commerce_hall":
                IReadOnlyList<string> commerceLines = _runtimeProfile.EnablePurchaseSimulation
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
                        "Сборка откроется в миссионной фазе Session 11.",
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
                        _runtimeProfile.EnableMissionPageDebugAccess
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
                    Lines = _runtimeProfile.UnlockHardArchivePreview
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
                _statusMessage = "Контур контроля архива обновлен.";
                AppendUiLog("[ARCHIVE] Контур контроля архива открыт.");
                WriteSessionLog("ARCHIVE", "Archive control opened");
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

    private void RecordBackgroundTelemetry()
    {
        if (_bundle is null)
            return;

        if (_bundle.HubRuntime.CurrentPhase != _lastLoggedPhase)
        {
            WriteSessionLog("STATE", $"Phase changed: {_lastLoggedPhase} -> {_bundle.HubRuntime.CurrentPhase}");
            _lastLoggedPhase = _bundle.HubRuntime.CurrentPhase;
        }

        var activeObjectiveId = _bundle.ObjectiveTracker.GetActive()?.ObjectiveId;
        if (!string.Equals(activeObjectiveId, _lastLoggedObjectiveId, StringComparison.Ordinal))
        {
            WriteSessionLog("STATE", $"Objective changed: {_lastLoggedObjectiveId ?? "<none>"} -> {activeObjectiveId ?? "<none>"}");
            _lastLoggedObjectiveId = activeObjectiveId;
        }

        var currentFocus = _focusedObjectId ?? _focusedExtraId;
        if (!string.Equals(currentFocus, _lastLoggedFocus, StringComparison.Ordinal))
        {
            if (!string.IsNullOrWhiteSpace(currentFocus))
                WriteSessionLog("FOCUS", currentFocus);
            _lastLoggedFocus = currentFocus;
        }

        var currentTerminal = _activeTerminal?.TerminalId;
        if (!string.Equals(currentTerminal, _lastLoggedTerminalId, StringComparison.Ordinal))
        {
            WriteSessionLog("OVERLAY", $"terminal={(currentTerminal ?? "<closed>")}");
            _lastLoggedTerminalId = currentTerminal;
        }

        if (_bundle.Session.Camera.ActiveMode != _lastLoggedCameraMode)
        {
            WriteSessionLog("CAMERA", $"mode={_bundle.Session.Camera.ActiveMode}");
            _lastLoggedCameraMode = _bundle.Session.Camera.ActiveMode;
        }

        if (_bundle.PrologueTracker.IsProtocolZeroUnlocked != _lastLoggedProtocolZero)
        {
            WriteSessionLog("STATE", $"ProtocolZero={_bundle.PrologueTracker.IsProtocolZeroUnlocked}");
            _lastLoggedProtocolZero = _bundle.PrologueTracker.IsProtocolZeroUnlocked;
        }

        if (!string.Equals(_statusMessage, _lastLoggedStatusMessage, StringComparison.Ordinal))
        {
            WriteSessionLog("STATUS", _statusMessage ?? "<none>");
            _lastLoggedStatusMessage = _statusMessage;
        }

        var nowMs = _clock.ElapsedMilliseconds;
        if (nowMs - _lastHeartbeatMs >= 1000)
        {
            var p = _bundle.Player.Position;
            WriteSessionLog(
                "TICK",
                $"phase={_bundle.HubRuntime.CurrentPhase}; objective={activeObjectiveId ?? "<none>"}; pos=({p.X:F2},{p.Y:F2},{p.Z:F2}); focus={currentFocus ?? "<none>"}; terminal={currentTerminal ?? "<closed>"}");
            _lastHeartbeatMs = nowMs;
        }
    }

    private void WriteSessionLog(string category, string message)
    {
        if (string.IsNullOrWhiteSpace(_sessionLogPath))
            return;

        try
        {
            var line = $"[{DateTimeOffset.Now:O}] [{category}] {message}{Environment.NewLine}";
            File.AppendAllText(_sessionLogPath, line, Encoding.UTF8);
        }
        catch
        {
            // Ignore file I/O failures to keep gameplay loop alive.
        }
    }

    private bool ConsumePressed(Keys key)
    {
        if (_keysPressed.Contains(key))
        {
            _keysPressed.Remove(key);
            return true;
        }
        return false;
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
