using BabylonArchiveCore.Core.Billing;
using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Economy;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Infrastructure.Billing;
using BabylonArchiveCore.Runtime.Economy;
using BabylonArchiveCore.Runtime.Progression;
using BabylonArchiveCore.Runtime.QA;

namespace BabylonArchiveCore.Tests;

public class DaySevenEconomyTests
{
    // ── Wallet ─────────────────────────────────────────────────────

    [Fact]
    public void Wallet_DefaultBalances_AreZero()
    {
        var w = new Wallet();
        Assert.Equal(0, w.Credits);
        Assert.Equal(0, w.Launs);
    }

    [Fact]
    public void Wallet_Earn_IncreasesBalance()
    {
        var w = new Wallet();
        w.Earn(CurrencyType.Credits, 100);
        Assert.Equal(100, w.Credits);
    }

    [Fact]
    public void Wallet_Earn_ZeroOrNegative_Ignored()
    {
        var w = new Wallet();
        w.Earn(CurrencyType.Credits, 0);
        w.Earn(CurrencyType.Credits, -5);
        Assert.Equal(0, w.Credits);
    }

    [Fact]
    public void Wallet_TrySpend_Succeeds_WhenAffordable()
    {
        var w = new Wallet();
        w.Earn(CurrencyType.Credits, 100);
        Assert.True(w.TrySpend(CurrencyType.Credits, 50));
        Assert.Equal(50, w.Credits);
    }

    [Fact]
    public void Wallet_TrySpend_Fails_WhenInsufficientFunds()
    {
        var w = new Wallet();
        w.Earn(CurrencyType.Credits, 10);
        Assert.False(w.TrySpend(CurrencyType.Credits, 50));
        Assert.Equal(10, w.Credits);
    }

    [Fact]
    public void Wallet_CanAfford_ChecksCorrectly()
    {
        var w = new Wallet();
        w.Earn(CurrencyType.Launs, 20);
        Assert.True(w.CanAfford(CurrencyType.Launs, 20));
        Assert.False(w.CanAfford(CurrencyType.Launs, 21));
    }

    // ── StoreItemDefinition pay-to-win compliance ──────────────────

    [Fact]
    public void StoreItem_Supply_WithLaunPrice_IsNotCompliant()
    {
        var item = new StoreItemDefinition
        {
            Id = "heal", Name = "Healing Kit", Description = "Heals",
            Category = StoreItemCategory.Supply,
            CreditPrice = 50, LaunPrice = 5,
        };
        Assert.False(item.IsPayToWinCompliant);
    }

    [Fact]
    public void StoreItem_Supply_WithoutLaunPrice_IsCompliant()
    {
        var item = new StoreItemDefinition
        {
            Id = "heal", Name = "Healing Kit", Description = "Heals",
            Category = StoreItemCategory.Supply,
            CreditPrice = 50,
        };
        Assert.True(item.IsPayToWinCompliant);
    }

    [Fact]
    public void StoreItem_Convenience_WithLaunPrice_IsCompliant()
    {
        var item = new StoreItemDefinition
        {
            Id = "fast_travel", Name = "Fast Travel Pass", Description = "Convenience",
            Category = StoreItemCategory.Convenience,
            CreditPrice = 200, LaunPrice = 10,
        };
        Assert.True(item.IsPayToWinCompliant);
    }

    // ── WalletRuntime ──────────────────────────────────────────────

    [Fact]
    public void WalletRuntime_Earn_PublishesEvent()
    {
        var (wrt, bus, _) = MakeWalletRuntime();
        CurrencyEarnedEvent? captured = null;
        bus.Subscribe<CurrencyEarnedEvent>(e => captured = e);

        wrt.Earn(CurrencyType.Credits, 75, "mission_reward");

        Assert.NotNull(captured);
        Assert.Equal(75, captured.Amount);
        Assert.Equal(CurrencyType.Credits, captured.Currency);
        Assert.Equal("mission_reward", captured.Source);
    }

    [Fact]
    public void WalletRuntime_TrySpend_Succeeds()
    {
        var (wrt, _, _) = MakeWalletRuntime();
        wrt.Earn(CurrencyType.Credits, 100, "test");
        Assert.True(wrt.TrySpend(CurrencyType.Credits, 30, "purchase"));
        Assert.Equal(70, wrt.Wallet.Credits);
    }

    [Fact]
    public void WalletRuntime_TrySpend_Fails_InsufficientFunds()
    {
        var (wrt, _, _) = MakeWalletRuntime();
        Assert.False(wrt.TrySpend(CurrencyType.Credits, 100, "overpriced"));
    }

    // ── StoreRuntime ───────────────────────────────────────────────

    [Fact]
    public void Store_RegisterItem_RejectsPayToWin()
    {
        var (store, _, _) = MakeStoreRuntime();
        var item = new StoreItemDefinition
        {
            Id = "pw", Name = "P2W Item", Description = "Bad",
            Category = StoreItemCategory.Supply,
            CreditPrice = 50, LaunPrice = 5,
        };
        Assert.False(store.RegisterItem(item));
        Assert.Empty(store.Catalog);
    }

    [Fact]
    public void Store_Purchase_Credits_Succeeds()
    {
        var (store, wallet, _) = MakeStoreRuntime();
        wallet.Earn(CurrencyType.Credits, 500);
        store.RegisterItem(MakeSupplyItem());

        var result = store.Purchase("supply_kit", CurrencyType.Credits, operatorLevel: 1);
        Assert.True(result.Success);
        Assert.Equal(400, wallet.Credits);
        Assert.Equal(1, store.GetOwnedCount("supply_kit"));
    }

    [Fact]
    public void Store_Purchase_InsufficientFunds_Fails()
    {
        var (store, _, _) = MakeStoreRuntime();
        store.RegisterItem(MakeSupplyItem());

        var result = store.Purchase("supply_kit", CurrencyType.Credits, operatorLevel: 1);
        Assert.False(result.Success);
        Assert.Contains("Insufficient", result.ErrorReason);
    }

    [Fact]
    public void Store_Purchase_LevelTooLow_Fails()
    {
        var (store, wallet, _) = MakeStoreRuntime();
        wallet.Earn(CurrencyType.Credits, 500);
        store.RegisterItem(new StoreItemDefinition
        {
            Id = "adv", Name = "Advanced", Description = "Needs level",
            Category = StoreItemCategory.Supply,
            CreditPrice = 50, RequiredLevel = 5,
        });

        var result = store.Purchase("adv", CurrencyType.Credits, operatorLevel: 2);
        Assert.False(result.Success);
        Assert.Contains("level", result.ErrorReason);
    }

    [Fact]
    public void Store_Purchase_MaxOwned_BlocksExcess()
    {
        var (store, wallet, _) = MakeStoreRuntime();
        wallet.Earn(CurrencyType.Credits, 500);
        store.RegisterItem(new StoreItemDefinition
        {
            Id = "limited", Name = "Limited", Description = "Max 1",
            Category = StoreItemCategory.Supply,
            CreditPrice = 50, MaxOwned = 1,
        });

        var r1 = store.Purchase("limited", CurrencyType.Credits, operatorLevel: 1);
        Assert.True(r1.Success);

        var r2 = store.Purchase("limited", CurrencyType.Credits, operatorLevel: 1);
        Assert.False(r2.Success);
        Assert.Contains("maximum", r2.ErrorReason);
    }

    [Fact]
    public void Store_Purchase_Launs_ForConvenience_Succeeds()
    {
        var (store, wallet, _) = MakeStoreRuntime();
        wallet.Earn(CurrencyType.Launs, 50);
        store.RegisterItem(new StoreItemDefinition
        {
            Id = "fast_travel", Name = "Fast Travel Pass", Description = "Convenience",
            Category = StoreItemCategory.Convenience,
            CreditPrice = 200, LaunPrice = 10,
        });

        var result = store.Purchase("fast_travel", CurrencyType.Launs, operatorLevel: 1);
        Assert.True(result.Success);
        Assert.Equal(40, wallet.Launs);
    }

    [Fact]
    public void Store_Purchase_Launs_ForSupply_Blocked()
    {
        var (store, wallet, _) = MakeStoreRuntime();
        wallet.Earn(CurrencyType.Credits, 500);
        wallet.Earn(CurrencyType.Launs, 50);
        store.RegisterItem(MakeSupplyItem());

        var result = store.Purchase("supply_kit", CurrencyType.Launs, operatorLevel: 1);
        Assert.False(result.Success);
        Assert.Contains("pay-to-win", result.ErrorReason);
    }

    [Fact]
    public void Store_Purchase_PublishesEvent()
    {
        var (store, wallet, bus) = MakeStoreRuntime();
        wallet.Earn(CurrencyType.Credits, 500);
        store.RegisterItem(MakeSupplyItem());

        PurchaseCompletedEvent? captured = null;
        bus.Subscribe<PurchaseCompletedEvent>(e => captured = e);

        store.Purchase("supply_kit", CurrencyType.Credits, operatorLevel: 1);
        Assert.NotNull(captured);
        Assert.Equal("supply_kit", captured.ItemId);
        Assert.Equal(100, captured.AmountCharged);
    }

    // ── FakeBillingProvider ────────────────────────────────────────

    [Fact]
    public async Task FakeBilling_ChargesLauns_WhenAvailable()
    {
        var wallet = new Wallet();
        wallet.Earn(CurrencyType.Launs, 100);
        var provider = new FakeBillingProvider(wallet);

        Assert.True(await provider.ChargeLaunsAsync("op1", 30, "item1"));
        Assert.Equal(70, wallet.Launs);
    }

    [Fact]
    public async Task FakeBilling_FailsCharge_WhenInsufficient()
    {
        var wallet = new Wallet();
        wallet.Earn(CurrencyType.Launs, 10);
        var provider = new FakeBillingProvider(wallet);

        Assert.False(await provider.ChargeLaunsAsync("op1", 50, "item1"));
        Assert.Equal(10, wallet.Launs);
    }

    [Fact]
    public async Task FakeBilling_ReturnsBalance()
    {
        var wallet = new Wallet();
        wallet.Earn(CurrencyType.Launs, 42);
        var provider = new FakeBillingProvider(wallet);

        Assert.Equal(42, await provider.GetLaunsBalanceAsync("op1"));
    }

    // ── SaveIntegrityValidator ─────────────────────────────────────

    [Fact]
    public void SaveValidator_ValidSave_Passes()
    {
        var save = new SaveGame { Version = 2 };
        var result = SaveIntegrityValidator.Validate(save, 2, new NullLogger());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void SaveValidator_VersionMismatch_Fails()
    {
        var save = new SaveGame { Version = 1 };
        var result = SaveIntegrityValidator.Validate(save, 2, new NullLogger());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("version"));
    }

    [Fact]
    public void SaveValidator_BadAddress_Fails()
    {
        var save = new SaveGame { Version = 2, WorldSeedAddress = "BAD" };
        var result = SaveIntegrityValidator.Validate(save, 2, new NullLogger());
        Assert.False(result.IsValid);
    }

    // ── BalanceChecker ─────────────────────────────────────────────

    [Fact]
    public void BalanceChecker_ValidMission_Passes()
    {
        var mission = MakeSampleMission();
        var curve = new BalanceCurve();
        var report = BalanceChecker.Check([mission], curve, new NullLogger());
        Assert.True(report.IsBalanced);
    }

    [Fact]
    public void BalanceChecker_NoMissions_Fails()
    {
        var curve = new BalanceCurve();
        var report = BalanceChecker.Check([], curve, new NullLogger());
        Assert.False(report.IsBalanced);
    }

    [Fact]
    public void BalanceChecker_MissingStartNode_ReportsIssue()
    {
        var mission = new MissionDefinition
        {
            Id = "broken", Title = "Broken", Type = MissionType.Main,
            StartNodeId = "nonexistent",
            Nodes = new()
            {
                ["intro"] = new MissionNode { Id = "intro", Description = "A" },
            },
        };
        var report = BalanceChecker.Check([mission], new BalanceCurve(), new NullLogger());
        Assert.False(report.IsBalanced);
        Assert.Contains(report.Issues, i => i.Contains("start node"));
    }

    // ── Helpers ────────────────────────────────────────────────────

    private static (WalletRuntime wrt, EventBus bus, ILogger log) MakeWalletRuntime()
    {
        var wallet = new Wallet();
        var bus = new EventBus();
        var log = new NullLogger();
        return (new WalletRuntime(wallet, bus, log), bus, log);
    }

    private static (StoreRuntime store, Wallet wallet, EventBus bus) MakeStoreRuntime()
    {
        var wallet = new Wallet();
        var bus = new EventBus();
        var log = new NullLogger();
        return (new StoreRuntime(wallet, bus, log), wallet, bus);
    }

    private static StoreItemDefinition MakeSupplyItem() => new()
    {
        Id = "supply_kit", Name = "Supply Kit", Description = "Basic supplies",
        Category = StoreItemCategory.Supply,
        CreditPrice = 100,
    };

    private static MissionDefinition MakeSampleMission() => new()
    {
        Id = "m_test", Title = "Test Mission", Type = MissionType.Main,
        StartNodeId = "start",
        Nodes = new()
        {
            ["start"] = new MissionNode
            {
                Id = "start", Description = "Begin",
                Transitions = new() { ["go"] = "end" },
            },
            ["end"] = new MissionNode { Id = "end", Description = "Done", IsTerminalSuccess = true },
        },
    };

    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }
}
