using BabylonArchiveCore.Core.Economy;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Economy;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session036CombatSmokeTests
{
    [Fact]
    public void WorldAxesAndEconomyBalance_Smoke()
    {
        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"s036\",\"damageMultipliers\":{\"player\":1.1},\"economy\":{\"inflationIndex\":1.2,\"buyPriceMultiplier\":1.05,\"sellPriceMultiplier\":0.9}}");

        var world = new WorldState();
        var economy = new EconomyState();
        var sync = new WorldEconomySynchronizer(economy, world);
        sync.ApplyBalanceProfile(table);

        var calculator = new DamageCalculator();
        var damage = calculator.CalculateDamageFromBalance(new DamageFormulaInput
        {
            AttackPower = 10,
            SkillMultiplier = 1,
            TargetArmor = 0,
            MinimumDamage = 1
        }, table);

        Assert.Equal(11, damage);
        Assert.Equal(1.2f, economy.InflationIndex);
    }
}
