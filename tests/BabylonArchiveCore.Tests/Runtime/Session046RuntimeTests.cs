using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Runtime.Migrations;
using BabylonArchiveCore.Runtime.Serialization;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime;

public class Session046RuntimeTests
{
    [Fact]
    public void ArchiveAddress_ParentAndNextPage_RoundTrip()
    {
        var address = ArchiveAddress.Parse("S2:H1:M0:Sh0:T0:P3");
        var parent = address.Parent();
        var roundTrip = parent.NextPage();

        Assert.Equal("S2:H1:M0:Sh0:T0:P2", parent.ToCanonicalString());
        Assert.Equal(address, roundTrip);
        Assert.Equal(6, address.ToPath().Count);
    }

    [Fact]
    public void ArchiveSeed_DeriveHierarchySeed_IsDeterministic()
    {
        var address = new ArchiveAddress(1, 1, 1, 1, 1, 1);
        var first = ArchiveSeed.DeriveHierarchySeed(address, worldSeed: 17);
        var second = ArchiveSeed.DeriveHierarchySeed(address, worldSeed: 17);
        var sibling = ArchiveSeed.DeriveHierarchySeed(address.NextPage(), worldSeed: 17);

        Assert.Equal(first, second);
        Assert.NotEqual(first, sibling);
    }

    [Fact]
    public void Migration046_AndSerializer_RoundTrip()
    {
        var migration = new Migration_046();
        var migrated = migration.Migrate(null);

        var serializer = new Session046Serializer();
        var json = serializer.Serialize(migrated);
        var restored = serializer.Deserialize(json);






}    }        Assert.Equal(6, restored.Path.Length);        Assert.Equal(46, restored.ContractVersion);
