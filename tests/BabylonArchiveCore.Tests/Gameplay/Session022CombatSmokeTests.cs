using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Combat.StatusEffects;
using BabylonArchiveCore.Runtime.Input;
using BabylonArchiveCore.Runtime.Inventory;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session022CombatSmokeTests
{
    [Fact]
    public void FullCombatCycle_DirectAoeStatusResolve()
    {
        var sem = new StatusEffectManager();
        sem.Apply(new StatusEffect { EffectId = "poison", Name = "Poison", RemainingTicks = 3, DamagePerTick = 2 });
        var cs = new CombatSystem(sem);
        var r1 = cs.ResolveTurn(10, 5);
        Assert.Equal(17, r1.TotalDamage);
        var r2 = cs.ResolveTurn(0, 0);
        Assert.Equal(2, r2.TotalDamage);
    }

    [Fact]
    public void InventoryAndCombatInput_Integration()
    {
        var inv = new InventoryManager();
        inv.TryAdd("sword", "Iron Sword");
        var input = new CombatInputHandler();
        input.SetTargets(new[] { "goblin", "orc" });
        Assert.Equal("goblin", input.CurrentTarget);
        input.TabTarget();
        Assert.Equal("orc", input.CurrentTarget);
        Assert.NotNull(inv.Find("sword"));
    }
}
