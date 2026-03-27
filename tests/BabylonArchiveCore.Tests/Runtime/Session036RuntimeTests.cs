using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using BabylonArchiveCore.Runtime.State;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session036RuntimeTests
{
    [Fact]
    public void WorldState_AxisDeltaAndSnapshot_Work()
    {
        var world = new WorldState();
        world.SetAxes(10f, -5f);
        world.ApplyAxisDelta(4f, -3f);

        var snapshot = world.GetAxisSnapshot();
        Assert.Equal(14f, snapshot.MoralAxis);
        Assert.Equal(-8f, snapshot.TechnoArcaneAxis);
        Assert.True(world.WorldAxisVersion >= 36);
    }

    [Fact]
    public void WorldStateService_AxisMethods_Work()
    {
        var world = new WorldState();
        var service = new WorldStateService(world);
        service.SetAxes(1f, 2f);
        service.ApplyAxisDelta(3f, -1f);

        var snapshot = service.GetAxisSnapshot();
        Assert.Equal(4f, snapshot.MoralAxis);
        Assert.Equal(1f, snapshot.TechnoArcaneAxis);
    }

    [Fact]
    public void Migration036_AndSerializer_RoundTrip()
    {
        var migration = new Migration_036();
        var migrated = migration.Migrate(null);
        Assert.Equal(36, migrated.WorldAxisVersion);

        var serializer = new Session036Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal(36, restored.WorldAxisVersion);
    }

    [Fact]
    public void DamageCalculator_CalculateDamageFromBalance_UsesMultiplier()
    {
        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"s036\",\"damageMultipliers\":{\"player\":1.2},\"damage\":{\"minDamage\":1}}");
        var calculator = new DamageCalculator();
        var value = calculator.CalculateDamageFromBalance(new DamageFormulaInput
        {
            AttackPower = 10,
            SkillMultiplier = 1,
            TargetArmor = 0,
            MinimumDamage = 0
        }, table);

        Assert.Equal(12, value);
    }
}
