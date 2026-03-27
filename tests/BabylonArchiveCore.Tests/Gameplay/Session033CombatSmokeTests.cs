using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Loot;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session033CombatSmokeTests
{
    [Fact]
    public void DamageAndLoot_Smoke()
    {
        var calculator = new DamageCalculator();
        var damage = calculator.CalculateDamage(new DamageFormulaInput
        {
            AttackPower = 15,
            SkillMultiplier = 1.2f,
            TargetArmor = 3,
            MinimumDamage = 1
        });

        var resolver = new DropResolver();
        var drop = resolver.ResolveDrop(
            new Dictionary<string, int>(StringComparer.Ordinal) { ["common"] = 90, ["rare"] = 10 },
            new[] { "herb", "ore", "crystal" },
            roll: 7,
            luckBias: 0.1f);

        Assert.True(damage >= 1);
        Assert.NotNull(drop.ItemId);
    }
}
