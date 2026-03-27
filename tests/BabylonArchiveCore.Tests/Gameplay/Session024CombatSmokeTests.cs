using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Combat.StatusEffects;
using BabylonArchiveCore.Runtime.Inventory;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session024CombatSmokeTests
{
    [Fact]
    public void Attributes_Inventory_Combat_Smoke()
    {
        var player = new PlayerState();
        player.SetStrength(25);
        player.SetAgility(18);

        var inventory = new InventoryManager(5);
        Assert.True(inventory.TryAdd("sword", "Iron Sword", 1));

        var status = new StatusEffectManager();
        status.Apply(new StatusEffect { EffectId = "bleed", Name = "Bleed", Category = "debuff", RemainingTicks = 1, DamagePerTick = 3 });

        var combat = new CombatSystem(status);
        var result = combat.ResolveTurn(10 + player.Strength / 5, 2);

        Assert.True(result.TotalDamage >= 15);
    }
}
