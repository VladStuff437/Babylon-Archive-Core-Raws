using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using BabylonArchiveCore.Runtime.State;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session035RuntimeTests
{
    [Fact]
    public void WorldState_ChangeFactionReputation_ClampsRange()
    {
        var world = new WorldState();
        var updated = world.ChangeFactionReputation("order", 300);

        Assert.Equal(100, updated);
        Assert.Equal(100, world.GetFactionReputation("order"));
    }

    [Fact]
    public void WorldStateService_SetAxes_ClampsValues()
    {
        var world = new WorldState();
        var service = new WorldStateService(world);
        service.SetAxes(500f, -500f);

        Assert.Equal(100f, world.MoralAxis);
        Assert.Equal(-100f, world.TechnoArcaneAxis);
    }

    [Fact]
    public void DamageCalculator_WithFactionContext_StillRespectsMinimum()
    {
        var calculator = new DamageCalculator();
        var damage = calculator.CalculateDamageWithContext(
            new DamageFormulaInput
            {
                AttackPower = 1,
                SkillMultiplier = 1,
                TargetArmor = 100,
                MinimumDamage = 2
            },
            new DamageContext
            {
                ReputationDelta = -200,
                MoralAxis = -50,
                TechnoArcaneAxis = -30
            });

        Assert.Equal(2, damage);
    }

    [Fact]
    public void Migration035_AndSerializer_RoundTrip()
    {
        var migration = new Migration_035();
        var migrated = migration.Migrate(null);

        var serializer = new Session035Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal(0f, restored.MoralAxis);
        Assert.Equal(0f, restored.TechnoArcaneAxis);
    }
}
