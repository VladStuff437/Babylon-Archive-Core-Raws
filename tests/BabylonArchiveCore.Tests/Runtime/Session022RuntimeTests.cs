using BabylonArchiveCore.Core.Gameplay.Interactions;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.AI.StateMachine;
using BabylonArchiveCore.Runtime.AI.Perception;
using BabylonArchiveCore.Runtime.Camera;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Combat.StatusEffects;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.Input;
using BabylonArchiveCore.Runtime.Inventory;
using BabylonArchiveCore.Runtime.State;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session022RuntimeTests
{
    // --- Camera ---
    [Fact]
    public void CameraController_DelegatesModeToCameraStateManager()
    {
        var sm = new CameraStateManager();
        var ctrl = new CameraController(sm);
        Assert.Equal("Follow", ctrl.CurrentMode);
        ctrl.SetMode("Aim");
        Assert.Equal("Aim", ctrl.CurrentMode);
    }

    // --- Interactions ---
    private sealed class TestInteractable : IInteractable
    {
        public string InteractableId { get; init; } = "obj-1";
        public string InteractionType => "Use";
        public bool Interacted { get; private set; }
        public bool CanInteract(string actorId) => true;
        public void OnInteract(string actorId) => Interacted = true;
    }

    [Fact]
    public void InteractionExecutor_Registers_And_Interacts()
    {
        var executor = new InteractionExecutor();
        var obj = new TestInteractable();
        executor.Register(obj);
        Assert.True(executor.TryInteract("player", "obj-1"));
        Assert.True(obj.Interacted);
    }

    [Fact]
    public void InteractionExecutor_ReturnsFalse_ForUnknownTarget()
    {
        var executor = new InteractionExecutor();
        Assert.False(executor.TryInteract("player", "nonexistent"));
    }

    // --- Inventory ---
    [Fact]
    public void InventoryManager_Add_And_Remove()
    {
        var inv = new InventoryManager(5);
        Assert.True(inv.TryAdd("potion", "Health Potion", 3));
        Assert.Equal(3, inv.Find("potion")!.Quantity);
        Assert.True(inv.TryRemove("potion", 2));
        Assert.Equal(1, inv.Find("potion")!.Quantity);
    }

    [Fact]
    public void InventoryManager_RespectsCapacity()
    {
        var inv = new InventoryManager(1);
        Assert.True(inv.TryAdd("a", "A"));
        Assert.False(inv.TryAdd("b", "B"));
    }

    [Fact]
    public void InventoryManager_RespectsStackLimit()
    {
        var inv = new InventoryManager(5);
        Assert.True(inv.TryAdd("x", "X", 5, 5));
        Assert.False(inv.TryAdd("x", "X", 1, 5)); // already at max stack
    }

    // --- PlayerState ---
    [Fact]
    public void PlayerState_ClampAttributes()
    {
        var ps = new PlayerState();
        ps.SetHealth(999);
        Assert.Equal(100, ps.Health); // clamped to MaxHealth
        ps.SetHealth(-10);
        Assert.Equal(0, ps.Health);
    }

    // --- StatusEffects ---
    [Fact]
    public void StatusEffectManager_TicksAndRemovesExpired()
    {
        var sem = new StatusEffectManager();
        sem.Apply(new StatusEffect { EffectId = "burn", Name = "Burn", RemainingTicks = 2, DamagePerTick = 5 });
        Assert.Single(sem.ActiveEffects);
        int d1 = sem.TickAll();
        Assert.Equal(5, d1);
        int d2 = sem.TickAll();
        Assert.Equal(5, d2);
        Assert.Empty(sem.ActiveEffects);
    }

    // --- CombatSystem ---
    [Fact]
    public void CombatSystem_ResolveTurn_OrderIsCorrect()
    {
        var sem = new StatusEffectManager();
        sem.Apply(new StatusEffect { EffectId = "dot", Name = "DOT", RemainingTicks = 1, DamagePerTick = 3 });
        var cs = new CombatSystem(sem);
        var result = cs.ResolveTurn(10, 5);
        Assert.Equal(10, result.DirectDamage);
        Assert.Equal(5, result.AoeDamage);
        Assert.Equal(3, result.StatusDamage);
        Assert.Equal(18, result.TotalDamage);
    }

    // --- AutoAttack ---
    [Fact]
    public void AutoAttackController_StartStop()
    {
        var aa = new AutoAttackController();
        aa.Start("enemy-1");
        Assert.True(aa.IsActive);
        Assert.Equal("enemy-1", aa.CurrentTargetId);
        aa.SafeStop();
        Assert.False(aa.IsActive);
        Assert.Null(aa.CurrentTargetId);
    }

    // --- CombatInputHandler ---
    [Fact]
    public void CombatInputHandler_TabAndEsc()
    {
        var handler = new CombatInputHandler();
        handler.SetTargets(new[] { "e1", "e2", "e3" });
        Assert.Equal("e1", handler.CurrentTarget);
        handler.TabTarget();
        Assert.Equal("e2", handler.CurrentTarget);
        handler.EscCancel();
        Assert.Null(handler.CurrentTarget);
    }

    // --- AI Perception ---
    [Fact]
    public void PerceptionSystem_DetectsWithinRadius()
    {
        var ps = new PerceptionSystem { DetectionRadius = 5f };
        var targets = ps.DetectTargets(0f, new[] { ("a", 3f), ("b", 10f) });
        Assert.Single(targets);
        Assert.Equal("a", targets[0]);
    }

    // --- AI StateMachine ---
    [Fact]
    public void AIStateMachine_TransitionsCorrectly()
    {
        var sm = new AIStateMachine();
        Assert.Equal(AIState.Idle, sm.Current);
        sm.Update(true, 1f, 100f);
        Assert.Equal(AIState.Attack, sm.Current);
        sm.Update(true, 5f, 100f);
        Assert.Equal(AIState.Chase, sm.Current);
        sm.Update(true, 5f, 10f);
        Assert.Equal(AIState.Flee, sm.Current);
        sm.Update(true, 5f, 0f);
        Assert.Equal(AIState.Dead, sm.Current);
    }
}
