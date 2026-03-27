using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Combat;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session032CombatSmokeTests
{
    [Fact]
    public void DamageCalculator_WithBalanceMultiplier_Smoke()
    {
        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"smoke\",\"damageMultipliers\":{\"player\":1.1}}");

        var calculator = new DamageCalculator();
        var damage = calculator.CalculateDamage(new DamageFormulaInput
        {
            AttackPower = 12,
            SkillMultiplier = 1,
            TargetArmor = 2,
            BalanceMultiplier = table.Scalars["damageMultipliers.player"],
            MinimumDamage = 1
        });

        Assert.Equal(11, damage);
    }
}
