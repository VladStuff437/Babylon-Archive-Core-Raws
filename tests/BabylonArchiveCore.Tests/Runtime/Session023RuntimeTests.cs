using BabylonArchiveCore.Core.Contracts;
using BabylonArchiveCore.Runtime.Inventory;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session023RuntimeTests
{
    [Fact]
    public void Migration023_ReturnsDefault_WhenNull()
    {
        var m = new Migration_023();
        var result = m.Migrate(null);
        Assert.Equal("player-1", result.OwnerId);
        Assert.Equal(20, result.Capacity);
        Assert.Empty(result.Slots);
    }

    [Fact]
    public void Migration023_PassesThrough_ExistingContract()
    {
        var existing = new Session023InventoryContract
        {
            OwnerId = "npc-1",
            Capacity = 10,
            Slots = new[]
            {
                new InventorySlot { ItemId = "gem", Name = "Ruby", Quantity = 5, MaxStack = 20 }
            }
        };
        var m = new Migration_023();
        var result = m.Migrate(existing);
        Assert.Same(existing, result);
    }

    [Fact]
    public void Session023Serializer_RoundTrip()
    {
        var contract = new Session023InventoryContract
        {
            OwnerId = "player-1",
            Capacity = 10,
            Slots = new[]
            {
                new InventorySlot { ItemId = "potion", Name = "Health Potion", Quantity = 3 }
            }
        };
        var ser = new Session023Serializer();
        var json = ser.Serialize(contract);
        var deserialized = ser.Deserialize(json);
        Assert.Equal("player-1", deserialized.OwnerId);
        Assert.Single(deserialized.Slots);
        Assert.Equal("potion", deserialized.Slots[0].ItemId);
    }

    [Fact]
    public void InventoryManager_FullCycle_AddStackRemove()
    {
        var inv = new InventoryManager(5);
        Assert.True(inv.TryAdd("potion", "Health Potion", 3, 10));
        Assert.True(inv.TryAdd("potion", "Health Potion", 5, 10));
        Assert.Equal(8, inv.Find("potion")!.Quantity);
        Assert.True(inv.TryRemove("potion", 8));
        Assert.Null(inv.Find("potion"));
    }
}
