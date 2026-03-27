using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.Combat.StatusEffects;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session025RuntimeTests
{
    [Fact]
    public void StatusEffect_Stacks_UpTo_Max()
    {
        var manager = new StatusEffectManager();
        manager.Apply(new StatusEffect { EffectId = "burn", Name = "Burn", Category = "debuff", RemainingTicks = 2, DamagePerTick = 2, MaxStacks = 3 });
        manager.Apply(new StatusEffect { EffectId = "burn", Name = "Burn", Category = "debuff", RemainingTicks = 2, DamagePerTick = 2, MaxStacks = 3 });
        manager.Apply(new StatusEffect { EffectId = "burn", Name = "Burn", Category = "debuff", RemainingTicks = 2, DamagePerTick = 2, MaxStacks = 3 });

        var active = manager.ActiveEffects.Single();
        Assert.Equal(3, active.StackCount);
    }

    [Fact]
    public void StatusEffectManager_RemoveByCategory_Works()
    {
        var manager = new StatusEffectManager();
        manager.Apply(new StatusEffect { EffectId = "poison", Name = "Poison", Category = "debuff", RemainingTicks = 2, DamagePerTick = 1 });
        manager.Apply(new StatusEffect { EffectId = "fortify", Name = "Fortify", Category = "buff", RemainingTicks = 2, DamagePerTick = 0, IsDebuff = false });

        var removed = manager.RemoveByCategory("debuff");

        Assert.Equal(1, removed);
        Assert.Single(manager.ActiveEffects);
    }

    [Fact]
    public void Migration025_And_Serializer_RoundTrip()
    {
        var migration = new Migration_025();
        var serializer = new Session025Serializer();

        var migrated = migration.Migrate(null);
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);

        Assert.Equal("player-1", restored.OwnerId);
        Assert.Empty(restored.Statuses);
    }
}
