using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Combat.StatusEffects;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session026RuntimeTests
{
    [Fact]
    public void CombatCore_Applies_Armor_And_Crit()
    {
        var system = new CombatSystem(new StatusEffectManager());

        var result = system.ResolveTurn(new CombatTurnContext
        {
            DirectBaseDamage = 20,
            AoeBaseDamage = 10,
            TargetArmor = 5,
            IsCritical = true,
            CriticalMultiplier = 2.0d
        });

        Assert.Equal(30, result.DirectDamage);
        Assert.Equal(5, result.AoeDamage);
        Assert.Equal(35, result.TotalDamage);
    }

    [Fact]
    public void Migration026_Returns_Default_When_Null()
    {
        var migration = new Migration_026();
        var result = migration.Migrate(null);

        Assert.Equal("player-1", result.AttackerId);
        Assert.Equal("target-default", result.TargetId);
    }

    [Fact]
    public void Session026Serializer_RoundTrip()
    {
        var serializer = new Session026Serializer();
        var payload = new Session026CombatCoreContract
        {
            AttackerId = "a",
            TargetId = "b",
            DirectBaseDamage = 11,
            AoeBaseDamage = 4,
            TargetArmor = 2,
            IsCritical = false
        };

        var json = serializer.Serialize(payload);
        var restored = serializer.Deserialize(json);

        Assert.Equal("a", restored.AttackerId);
        Assert.Equal(11, restored.DirectBaseDamage);
    }
}
