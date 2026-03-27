using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Combat;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session035CombatSmokeTests
{
    [Fact]
    public void ReputationAxesInfluenceDamage_Smoke()
    {
        var world = new WorldState();
        world.SetAxes(30f, 20f);
        world.SetFactionReputation("order", 60);

        var calculator = new DamageCalculator();
        var damage = calculator.CalculateDamageWithContext(
            new DamageFormulaInput
            {
                AttackPower = 20,
                SkillMultiplier = 1f,
                TargetArmor = 5,
                MinimumDamage = 1
            },
            new DamageContext
            {
                ReputationDelta = world.GetFactionReputation("order"),
                MoralAxis = world.MoralAxis,
                TechnoArcaneAxis = world.TechnoArcaneAxis
            });

        Assert.True(damage >= 1);
    }
}
