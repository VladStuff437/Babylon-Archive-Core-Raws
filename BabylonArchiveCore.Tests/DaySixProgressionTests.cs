using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Domain.Progression;
using BabylonArchiveCore.Runtime.Progression;

namespace BabylonArchiveCore.Tests;

public class DaySixProgressionTests
{
    // ── Balance Curve ──────────────────────────────────────────────

    [Fact]
    public void BalanceCurve_Level1_RequiresZeroXp()
    {
        var curve = new BalanceCurve();
        Assert.Equal(0, curve.XpForLevel(1));
    }

    [Fact]
    public void BalanceCurve_Level2_RequiresBaseXp()
    {
        var curve = new BalanceCurve { BaseXp = 100 };
        Assert.Equal(100, curve.XpForLevel(2));
    }

    [Fact]
    public void BalanceCurve_HigherLevels_GrowPolynomially()
    {
        var curve = new BalanceCurve { BaseXp = 100, GrowthExponent = 1.5 };
        var xp3 = curve.XpForLevel(3);
        var xp4 = curve.XpForLevel(4);
        Assert.True(xp3 > 100, "Level 3 should cost more than base");
        Assert.True(xp4 > xp3, "Level 4 should cost more than level 3");
    }

    [Fact]
    public void BalanceCurve_RecommendedLevelForTome_Scaling()
    {
        var curve = new BalanceCurve();
        Assert.Equal(1, curve.RecommendedLevelForTome(1));
        Assert.Equal(3, curve.RecommendedLevelForTome(2));
        Assert.Equal(6, curve.RecommendedLevelForTome(3));
        Assert.Equal(10, curve.RecommendedLevelForTome(4));
        Assert.Equal(15, curve.RecommendedLevelForTome(5));
    }

    [Fact]
    public void BalanceCurve_XpForTome_MatchesRecommendedLevel()
    {
        var curve = new BalanceCurve();
        Assert.Equal(curve.XpForLevel(6), curve.XpForTome(3));
    }

    // ── OperatorProfile ────────────────────────────────────────────

    [Fact]
    public void OperatorProfile_DefaultStats_AllTen()
    {
        var profile = new OperatorProfile();
        foreach (StatType stat in Enum.GetValues<StatType>())
            Assert.Equal(10, profile.GetStat(stat));
    }

    [Fact]
    public void OperatorProfile_AddStat_IncreasesStat()
    {
        var profile = new OperatorProfile();
        profile.AddStat(StatType.Analysis, 5);
        Assert.Equal(15, profile.GetStat(StatType.Analysis));
    }

    [Fact]
    public void OperatorProfile_AddStat_DoesNotGoBelowZero()
    {
        var profile = new OperatorProfile();
        profile.AddStat(StatType.Tech, -100);
        Assert.Equal(0, profile.GetStat(StatType.Tech));
    }

    // ── ProgressionRuntime — XP & Leveling ─────────────────────────

    [Fact]
    public void AwardXp_BelowThreshold_NoLevelUp()
    {
        var (rt, _) = MakeRuntime();
        rt.AwardXp(50);
        Assert.Equal(1, rt.Profile.Level);
        Assert.Equal(50, rt.Profile.Experience);
    }

    [Fact]
    public void AwardXp_ExactThreshold_LevelsUp()
    {
        var (rt, _) = MakeRuntime();
        rt.AwardXp(100);
        Assert.Equal(2, rt.Profile.Level);
    }

    [Fact]
    public void AwardXp_PublishesLevelUpEvent()
    {
        var (rt, bus) = MakeRuntime();
        LevelUpEvent? captured = null;
        bus.Subscribe<LevelUpEvent>(e => captured = e);
        rt.AwardXp(100);
        Assert.NotNull(captured);
        Assert.Equal(1, captured.OldLevel);
        Assert.Equal(2, captured.NewLevel);
    }

    [Fact]
    public void AwardXp_GrantsStatPoints()
    {
        var (rt, _) = MakeRuntime();
        rt.AwardXp(100);
        Assert.Equal(3, rt.Profile.StatPointsAvailable);
    }

    [Fact]
    public void AwardXp_MultiplelevelsAtOnce()
    {
        var (rt, _) = MakeRuntime();
        // XpForLevel(3) with exponent 1.5 = (int)(100 * 2^1.5) = 282
        rt.AwardXp(300);
        Assert.True(rt.Profile.Level >= 3, $"With 300 XP, expected level ≥ 3, got {rt.Profile.Level}");
    }

    [Fact]
    public void AwardXp_ZeroOrNegative_Ignored()
    {
        var (rt, _) = MakeRuntime();
        rt.AwardXp(0);
        rt.AwardXp(-10);
        Assert.Equal(0, rt.Profile.Experience);
    }

    // ── ProgressionRuntime — Stat Allocation ───────────────────────

    [Fact]
    public void AllocateStat_WithPoints_Succeeds()
    {
        var (rt, _) = MakeRuntime();
        rt.AwardXp(100); // level 2, 3 stat points
        var ok = rt.AllocateStat(StatType.Analysis, 2);
        Assert.True(ok);
        Assert.Equal(12, rt.Profile.GetStat(StatType.Analysis));
        Assert.Equal(1, rt.Profile.StatPointsAvailable);
    }

    [Fact]
    public void AllocateStat_NotEnoughPoints_Fails()
    {
        var (rt, _) = MakeRuntime();
        var ok = rt.AllocateStat(StatType.Analysis);
        Assert.False(ok);
    }

    // ── ProgressionRuntime — Perks ────────────────────────────────

    [Fact]
    public void UnlockPerk_MeetsRequirements_Succeeds()
    {
        var (rt, _) = MakeRuntime();
        rt.RegisterPerk(new PerkDefinition
        {
            Id = "p1",
            Name = "Deep Scan",
            Description = "Scan deeper",
            RequiredLevel = 2,
            GrantsCapability = "deep_scan",
        });
        rt.AwardXp(100); // level 2
        Assert.True(rt.TryUnlockPerk("p1"));
        Assert.Contains("p1", rt.Profile.UnlockedPerks);
    }

    [Fact]
    public void UnlockPerk_LevelTooLow_Fails()
    {
        var (rt, _) = MakeRuntime();
        rt.RegisterPerk(new PerkDefinition
        {
            Id = "p1",
            Name = "Deep Scan",
            Description = "Scan deeper",
            RequiredLevel = 5,
            GrantsCapability = "deep_scan",
        });
        Assert.False(rt.TryUnlockPerk("p1"));
    }

    [Fact]
    public void UnlockPerk_MissingPrereq_Fails()
    {
        var (rt, _) = MakeRuntime();
        rt.RegisterPerk(new PerkDefinition
        {
            Id = "p2",
            Name = "Advanced Scan",
            Description = "Even deeper",
            PrerequisitePerks = ["p1"],
            GrantsCapability = "adv_scan",
        });
        Assert.False(rt.TryUnlockPerk("p2"));
    }

    [Fact]
    public void GetCapabilities_ReturnsUnlockedPerkCapabilities()
    {
        var (rt, _) = MakeRuntime();
        rt.RegisterPerk(new PerkDefinition
        {
            Id = "p1",
            Name = "Deep Scan",
            Description = "Scan deeper",
            RequiredLevel = 1,
            GrantsCapability = "deep_scan",
        });
        rt.TryUnlockPerk("p1");
        var caps = rt.GetCapabilities();
        Assert.Contains("deep_scan", caps);
    }

    // ── SchematicRegistry ─────────────────────────────────────────

    [Fact]
    public void CollectFragment_NewFragment_ReturnsTrue()
    {
        var reg = new SchematicRegistry();
        var frag = new SchematicFragment { Id = "f1", SchematicId = "s1", Description = "Part A" };
        Assert.True(reg.CollectFragment(frag));
        Assert.Equal(1, reg.FragmentCount("s1"));
    }

    [Fact]
    public void CollectFragment_Duplicate_ReturnsFalse()
    {
        var reg = new SchematicRegistry();
        var frag = new SchematicFragment { Id = "f1", SchematicId = "s1", Description = "Part A" };
        reg.CollectFragment(frag);
        Assert.False(reg.CollectFragment(frag));
    }

    [Fact]
    public void TryComplete_AllRequirements_Succeeds()
    {
        var reg = new SchematicRegistry();
        reg.RegisterSchematic(new SchematicDefinition
        {
            Id = "s1",
            Name = "Fractal Decoder",
            Description = "Decodes fractal patterns",
            RequiredFragments = 2,
            GrantsCapability = "fractal_decode",
        });
        reg.CollectFragment(new SchematicFragment { Id = "f1", SchematicId = "s1", Description = "Part A" });
        reg.CollectFragment(new SchematicFragment { Id = "f2", SchematicId = "s1", Description = "Part B" });

        var profile = new OperatorProfile();
        Assert.True(reg.TryComplete("s1", profile));
        Assert.Contains("s1", reg.CompletedSchematics);
    }

    [Fact]
    public void TryComplete_NotEnoughFragments_Fails()
    {
        var reg = new SchematicRegistry();
        reg.RegisterSchematic(new SchematicDefinition
        {
            Id = "s1",
            Name = "Fractal Decoder",
            Description = "Decodes fractal patterns",
            RequiredFragments = 3,
            GrantsCapability = "fractal_decode",
        });
        reg.CollectFragment(new SchematicFragment { Id = "f1", SchematicId = "s1", Description = "Part A" });

        Assert.False(reg.TryComplete("s1", new OperatorProfile()));
    }

    [Fact]
    public void TryComplete_StatRequirementNotMet_Fails()
    {
        var reg = new SchematicRegistry();
        reg.RegisterSchematic(new SchematicDefinition
        {
            Id = "s1",
            Name = "Fractal Decoder",
            Description = "Decode",
            RequiredFragments = 1,
            RequiredStats = new() { [StatType.Tech] = 20 },
            GrantsCapability = "decode",
        });
        reg.CollectFragment(new SchematicFragment { Id = "f1", SchematicId = "s1", Description = "Part A" });

        Assert.False(reg.TryComplete("s1", new OperatorProfile()));
    }

    [Fact]
    public void GetCapabilities_ReturnsCompletedSchematicCapabilities()
    {
        var reg = new SchematicRegistry();
        reg.RegisterSchematic(new SchematicDefinition
        {
            Id = "s1",
            Name = "Decoder",
            Description = "Decode",
            RequiredFragments = 1,
            GrantsCapability = "fractal_decode",
        });
        reg.CollectFragment(new SchematicFragment { Id = "f1", SchematicId = "s1", Description = "A" });
        reg.TryComplete("s1", new OperatorProfile());

        Assert.Contains("fractal_decode", reg.GetCapabilities());
    }

    // ── ProgressionGateResolver ───────────────────────────────────

    [Fact]
    public void Gate_MinLevel_Pass()
    {
        var gate = new ProgressionGate
        {
            Id = "g1",
            TargetAddress = "A-0:1:2",
            Requirements = [new() { Type = ProgressionGateType.MinLevel, Key = "", Threshold = 3 }],
        };
        var profile = new OperatorProfile { Level = 5 };
        Assert.True(ProgressionGateResolver.CanPass(gate, profile, new()));
    }

    [Fact]
    public void Gate_MinLevel_Fail()
    {
        var gate = new ProgressionGate
        {
            Id = "g1",
            TargetAddress = "A-0:1:2",
            Requirements = [new() { Type = ProgressionGateType.MinLevel, Key = "", Threshold = 10 }],
        };
        var profile = new OperatorProfile { Level = 3 };
        Assert.False(ProgressionGateResolver.CanPass(gate, profile, new()));
    }

    [Fact]
    public void Gate_CapabilityRequired_Pass()
    {
        var gate = new ProgressionGate
        {
            Id = "g2",
            TargetAddress = "A-0:3:1",
            Requirements = [new() { Type = ProgressionGateType.CapabilityRequired, Key = "deep_scan" }],
        };
        Assert.True(ProgressionGateResolver.CanPass(gate, new OperatorProfile(), new(["deep_scan"])));
    }

    [Fact]
    public void Gate_MultipleRequirements_AllMustPass()
    {
        var gate = new ProgressionGate
        {
            Id = "g3",
            TargetAddress = "A-0:5:0",
            Requirements =
            [
                new() { Type = ProgressionGateType.MinLevel, Key = "", Threshold = 3 },
                new() { Type = ProgressionGateType.CapabilityRequired, Key = "fractal_decode" },
            ],
        };
        var profile = new OperatorProfile { Level = 5 };
        // Missing capability
        Assert.False(ProgressionGateResolver.CanPass(gate, profile, new()));
        // Both met
        Assert.True(ProgressionGateResolver.CanPass(gate, profile, new(["fractal_decode"])));
    }

    // ── Helper ─────────────────────────────────────────────────────

    private static (ProgressionRuntime rt, EventBus bus) MakeRuntime()
    {
        var profile = new OperatorProfile();
        var bus = new EventBus();
        var curve = new BalanceCurve { BaseXp = 100, GrowthExponent = 1.5 };
        return (new ProgressionRuntime(profile, bus, curve), bus);
    }
}
