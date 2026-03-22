using BabylonArchiveCore.Core.Camera;
using BabylonArchiveCore.Core.Input;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Player;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Per-frame gameplay orchestrator. Processes input, moves player, updates camera,
/// and dispatches interactions to HubA0Runtime + InteractionHandler.
/// </summary>
public sealed class GameplaySession
{
    private readonly PlayerController _playerCtrl;
    private readonly CameraController _camera;
    private readonly HubA0Runtime _hubRuntime;
    private readonly InteractionHandler? _interactionHandler;
    private readonly ILogger _logger;

    public PlayerController PlayerCtrl => _playerCtrl;
    public CameraController Camera => _camera;
    public HubA0Runtime HubRuntime => _hubRuntime;
    public InteractionHandler? InteractionHandler => _interactionHandler;
    public PlayerEntity Player => _playerCtrl.Player;
    public int FrameCount { get; private set; }

    public GameplaySession(
        PlayerEntity player,
        SceneGeometry geometry,
        CollisionSystem3D collision,
        HubA0Runtime hubRuntime,
        CameraProfile initialCameraProfile,
        ILogger logger,
        InteractionHandler? interactionHandler = null)
    {
        _playerCtrl = new PlayerController(player, collision, geometry);
        _camera = new CameraController(initialCameraProfile, player.Position);
        _hubRuntime = hubRuntime;
        _interactionHandler = interactionHandler;
        _logger = logger;

        _logger.Info($"GameplaySession started. Player at ({player.Position.X:F1},{player.Position.Z:F1}), camera mode={initialCameraProfile.Mode}");
    }

    /// <summary>
    /// Process a single frame: movement, camera, and interactions.
    /// Returns the frame result describing what happened.
    /// </summary>
    public FrameResult ProcessFrame(InputSnapshot input, float deltaTime)
    {
        FrameCount++;

        // 1. Camera toggle
        if (input.CameraTogglePressed)
        {
            _camera.ToggleMode();
            _logger.Info($"Camera toggled to {_camera.ActiveMode}");
        }

        // 2. Movement
        var moveResult = _playerCtrl.ProcessMovement(input, deltaTime);

        // 3. Camera follow
        _camera.Update(Player.Position, deltaTime);

        // 4. Zone tracking
        var currentZone = _playerCtrl.GetCurrentZone();

        // 5. Interaction
        InteractionResult? interaction = null;
        if (input.InteractPressed)
        {
            var target = _playerCtrl.TryGetInteractionTarget(_hubRuntime.CurrentPhase);
            if (target is { InRange: true })
            {
                var baseResult = _hubRuntime.Interact(target.Value.ObjectId);

                // Enrich via InteractionHandler if available
                if (_interactionHandler is not null)
                {
                    var obj = _hubRuntime.Zones
                        .SelectMany(z => z.Objects)
                        .FirstOrDefault(o => o.Id == target.Value.ObjectId);

                    interaction = obj is not null
                        ? _interactionHandler.Handle(obj, baseResult)
                        : baseResult;
                }
                else
                {
                    interaction = baseResult;
                }

                _logger.Info($"Interaction: {target.Value.ObjectId} → {interaction.Message}");
            }
        }

        return new FrameResult
        {
            Frame = FrameCount,
            PlayerPosition = Player.Position,
            PlayerFacing = Player.Facing,
            IsMoving = Player.IsMoving,
            CameraPosition = _camera.Position,
            CameraMode = _camera.ActiveMode,
            CurrentZone = currentZone?.Id,
            MoveClamped = moveResult.WasClamped,
            MoveBlocked = moveResult.WasBlocked,
            Interaction = interaction,
            Phase = _hubRuntime.CurrentPhase,
        };
    }
}

/// <summary>
/// Result of processing a single gameplay frame.
/// </summary>
public sealed record FrameResult
{
    public int Frame { get; init; }
    public Vec3 PlayerPosition { get; init; }
    public Vec3 PlayerFacing { get; init; }
    public bool IsMoving { get; init; }
    public Vec3 CameraPosition { get; init; }
    public CameraMode CameraMode { get; init; }
    public HubZoneId? CurrentZone { get; init; }
    public bool MoveClamped { get; init; }
    public bool MoveBlocked { get; init; }
    public InteractionResult? Interaction { get; init; }
    public HubRhythmPhase Phase { get; init; }
}
