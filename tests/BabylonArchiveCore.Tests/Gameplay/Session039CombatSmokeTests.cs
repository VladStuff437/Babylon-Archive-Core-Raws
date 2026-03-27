using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Loot;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session039CombatSmokeTests
{
    [Fact]
    public void BalanceDrivenDamageAndLoot_Smoke()
    {
        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"s039\",\"damageMultipliers\":{\"player\":1.04},\"loot\":{\"luckBias\":0.18},\"lootRarityWeights\":{\"common\":55,\"rare\":12,\"legendary\":2}}");

        var calculator = new DamageCalculator();
        var damage = calculator.CalculateDamageFromBalance(new DamageFormulaInput
        {
            AttackPower = 11,
            SkillMultiplier = 1,
            TargetArmor = 0,
            MinimumDamage = 1
        }, table);

        var resolver = new DropResolver();
        var drop = resolver.ResolveDropFromBalance(table, new[] { "item.a", "item.b", "item.c" }, 57);

        Assert.True(damage >= 1);
        Assert.Contains(drop.ItemId, new[] { "item.a", "item.b", "item.c" });
    }
}
