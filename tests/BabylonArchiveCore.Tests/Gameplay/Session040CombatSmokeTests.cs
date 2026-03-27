using BabylonArchiveCore.Core.Economy;
using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Combat;
using BabylonArchiveCore.Runtime.Economy;
using BabylonArchiveCore.Runtime.Missions;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session040CombatSmokeTests
{
    [Fact]
    public void MissionRuntimeWithEconomyAndDamage_Smoke()
    {
        var loader = new BalanceTableLoader();
        var table = loader.LoadFromJson("{\"profileId\":\"s040\",\"mission\":{\"priorityWeight\":1,\"satisfiedBonus\":100,\"fallbackPenalty\":500},\"economy\":{\"inflationIndex\":1.08,\"buyPriceMultiplier\":1.02,\"sellPriceMultiplier\":0.92},\"damageMultipliers\":{\"player\":1.05}}");

        var world = new WorldState();
        var economy = new EconomyState();
        var sync = new WorldEconomySynchronizer(economy, world);
        sync.ApplyBalanceProfile(table);

        var definition = new MissionDefinition
        {
            MissionId = "mission-040",
            Title = "Runtime Smoke",
            StartNodeId = "start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "start",
                    Description = "Start",
                    IsTerminal = false,
                    IsCheckpoint = true,
                    Transitions = new[]
                    {
                        new MissionTransition { TargetNodeId = "success", Priority = 2, ConditionKey = "cond.ok", IsFallback = false },
                        new MissionTransition { TargetNodeId = "fallback", Priority = 100, ConditionKey = "cond.ok", IsFallback = true }
                    }
                },
                new MissionNode { NodeId = "success", Description = "Success", IsTerminal = true, IsCheckpoint = false, Transitions = Array.Empty<MissionTransition>() },
                new MissionNode { NodeId = "fallback", Description = "Fallback", IsTerminal = true, IsCheckpoint = false, Transitions = Array.Empty<MissionTransition>() }
            }
        };

        var runtime = new MissionRuntimeEngine();
        var state = runtime.Start(definition);
        var next = runtime.Advance(definition, state, new[] { "cond.ok" }, table);

        var calculator = new DamageCalculator();
        var damage = calculator.CalculateDamageFromBalance(new DamageFormulaInput
        {
            AttackPower = 10,
            SkillMultiplier = 1,
            TargetArmor = 0,
            MinimumDamage = 1
        }, table);

        Assert.NotNull(next);
        Assert.Equal("success", next!.NodeId);
        Assert.Equal(1.08f, economy.InflationIndex);
        Assert.Equal(11, damage);
    }
}
