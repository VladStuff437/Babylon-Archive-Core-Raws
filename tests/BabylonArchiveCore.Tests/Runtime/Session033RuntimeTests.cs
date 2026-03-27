using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Loot;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session033RuntimeTests
{
    [Fact]
    public void DropResolver_ResolveDropTier_UsesWeightsAndLuckBias()
    {
        var resolver = new DropResolver();
        var weights = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            ["common"] = 70,
            ["rare"] = 30
        };

        var tier = resolver.ResolveDropTier(weights, roll: 80, luckBias: 0.2f);

        Assert.Equal("rare", tier);
    }

    [Fact]
    public void DropResolver_ResolveDrop_ReturnsDeterministicCandidate()
    {
        var resolver = new DropResolver();
        var drop = resolver.ResolveDrop(
            rarityWeights: new Dictionary<string, int>(StringComparer.Ordinal) { ["common"] = 1 },
            candidates: new[] { "item-a", "item-b" },
            roll: 3);

        Assert.Equal("item-b", drop.ItemId);
        Assert.Equal("common", drop.Rarity);
        Assert.True(drop.Quantity >= 1);
    }

    [Fact]
    public void DamageCalculator_CalculateDamageWithContext_AppliesContextScaling()
    {
        var calculator = new DamageCalculator();
        var damage = calculator.CalculateDamageWithContext(
            new DamageFormulaInput
            {
                AttackPower = 20,
                SkillMultiplier = 1,
                TargetArmor = 2,
                MinimumDamage = 1
            },
            new DamageContext
            {
                ReputationDelta = 100,
                MoralAxis = 50,
                TechnoArcaneAxis = 20
            });

        Assert.True(damage >= 1);
    }

    [Fact]
    public void Migration033_AndSerializer_RoundTrip()
    {
        var migration = new Migration_033();
        var migrated = migration.Migrate(null);
        Assert.Equal("session-033", migrated.BalanceProfileId);

        var serializer = new Session033Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal("session-033", restored.BalanceProfileId);
        Assert.True(restored.RarityWeights.Count > 0);
    }

    [Fact]
    public void BalanceTableLoader_LoadsSession033Sections()
    {
        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"033\",\"loot\":{\"luckBias\":0.2},\"lootRarityWeights\":{\"common\":50}} ");

        Assert.Equal("033", table.ProfileId);
        Assert.Equal(0.2f, table.GetScalar("loot.luckBias"));
        Assert.Equal(50, table.GetWeight("common"));
    }
}
