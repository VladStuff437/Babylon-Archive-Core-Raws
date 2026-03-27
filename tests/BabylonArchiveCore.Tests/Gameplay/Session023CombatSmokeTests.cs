using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Combat.StatusEffects;
using BabylonArchiveCore.Runtime.Inventory;
using BabylonArchiveCore.Runtime.AI.StateMachine;
using BabylonArchiveCore.Runtime.AI.Perception;
using BabylonArchiveCore.Runtime.Input;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session023CombatSmokeTests
{
    [Fact]
    public void FullGameplayLoop_Inventory_Combat_AI()
    {
        // Inventory
        var inv = new InventoryManager(10);
        inv.TryAdd("sword", "Iron Sword");
        inv.TryAdd("potion", "Health Potion", 5);
        Assert.Equal(2, inv.Items.Count);

        // Combat
        var sem = new StatusEffectManager();
        sem.Apply(new StatusEffect { EffectId = "bleed", Name = "Bleed", RemainingTicks = 2, DamagePerTick = 4 });
        var cs = new CombatSystem(sem);
        var r = cs.ResolveTurn(15, 3);
        Assert.Equal(22, r.TotalDamage);

        // AI
        var ai = new AIStateMachine();
        ai.Update(true, 1.5f, 80f);
        Assert.Equal(AIState.Attack, ai.Current);

        // Perception
        var perception = new PerceptionSystem { DetectionRadius = 8f };
        var detected = perception.DetectTargets(0f, new[] { ("enemy-1", 7f), ("enemy-2", 20f) });
        Assert.Single(detected);

        // Combat Input
        var handler = new CombatInputHandler();
        handler.SetTargets(new[] { "enemy-1" });
        Assert.Equal("enemy-1", handler.CurrentTarget);

        // Auto attack
        var aa = new AutoAttackController();
        aa.Start("enemy-1");
        Assert.True(aa.IsActive);
        aa.SafeStop();
        Assert.False(aa.IsActive);
    }
}
