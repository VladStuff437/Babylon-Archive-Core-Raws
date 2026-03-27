using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Combat.StatusEffects;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session025CombatSmokeTests
{
    [Fact]
    public void StatusStacking_Affects_CombatDamage()
    {
        var status = new StatusEffectManager();
        status.Apply(new StatusEffect { EffectId = "burn", Name = "Burn", Category = "debuff", RemainingTicks = 2, DamagePerTick = 2, MaxStacks = 3 });
        status.Apply(new StatusEffect { EffectId = "burn", Name = "Burn", Category = "debuff", RemainingTicks = 2, DamagePerTick = 2, MaxStacks = 3 });

        var combat = new CombatSystem(status);
        var result = combat.ResolveTurn(5, 0);

        Assert.Equal(9, result.TotalDamage);
        Assert.True(status.HasEffect("burn"));
    }
}
