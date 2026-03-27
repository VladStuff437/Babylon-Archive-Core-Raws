using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Loot;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session031RuntimeTests
{
    [Fact]
    public void DamageCalculator_HandlesBoundaryCases()
    {
        var calculator = new DamageCalculator();

        var zeroed = calculator.CalculateDamage(new DamageFormulaInput
        {
            AttackPower = -100f,
            SkillMultiplier = -3f,
            TargetArmor = 999f,
            MinimumDamage = 0
        });
        Assert.Equal(0, zeroed);

        var critical = calculator.CalculateDamage(new DamageFormulaInput
        {
            AttackPower = 10f,
            SkillMultiplier = 1f,
            TargetArmor = 2f,
            ForceCritical = true,
            CriticalMultiplier = 2f
        });
        Assert.Equal(16, critical);
    }

    [Fact]
    public void DropResolver_UsesWeightsDeterministically()
    {
        var resolver = new DropResolver();
        var rarity = resolver.ResolveRarity(new Dictionary<string, int>
        {
            ["common"] = 60,
            ["rare"] = 40
        }, 75);

        Assert.Equal("rare", rarity);
    }

    [Fact]
    public void BalanceTableLoader_LoadsSession031Shape()
    {
        var loader = new BalanceTableLoader();
        var json = "{\"profileId\":\"s031\",\"damage\":{\"baseAttackPower\":12},\"lootRarityWeights\":{\"common\":60}}";

        var table = loader.LoadFromJson(json);

        Assert.Equal("s031", table.ProfileId);
        Assert.Equal(12f, table.Scalars["damage.baseAttackPower"]);
        Assert.Equal(60, table.Weights["common"]);
    }

    [Fact]
    public void Migration031_AndSerializer_RoundTrip()
    {
        var migration = new Migration_031();
        var migrated = migration.Migrate(null);
        Assert.Equal("default", migrated.FormulaId);

        var serializer = new Session031Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);
        Assert.Equal("default", restored.FormulaId);
    }
}
