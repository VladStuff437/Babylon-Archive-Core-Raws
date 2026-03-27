using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Combat.StatusEffects;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session026CombatSmokeTests
{
    [Fact]
    public void CombatCore_ContextFlow_Smoke()
    {
        var status = new StatusEffectManager();
        status.Apply(new StatusEffect { EffectId = "dot", Name = "DOT", Category = "debuff", RemainingTicks = 1, DamagePerTick = 2 });

        var combat = new CombatSystem(status);
        var result = combat.ResolveTurn(new CombatTurnContext
        {
            DirectBaseDamage = 12,
            AoeBaseDamage = 6,
            TargetArmor = 3,
            IsCritical = true,
            CriticalMultiplier = 1.5d
        });

        Assert.Equal(19, result.TotalDamage);
    }
}
