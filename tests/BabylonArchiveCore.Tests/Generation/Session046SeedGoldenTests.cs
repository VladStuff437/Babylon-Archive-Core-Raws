using BabylonArchiveCore.Core.Archive;
using Xunit;

namespace BabylonArchiveCore.Tests.Generation;

public class Session046SeedGoldenTests
{
    [Fact]
    public void DeriveHierarchySeed_ChangesAcrossParentChildAddresses()
    {
        var address = new ArchiveAddress(6, 2, 1, 0, 0, 3);
        var parent = address.Parent();

        var childSeed = ArchiveSeed.DeriveHierarchySeed(address, worldSeed: 46);
        var parentSeed = ArchiveSeed.DeriveHierarchySeed(parent, worldSeed: 46);

        Assert.NotEqual(childSeed, parentSeed);
    }

    [Fact]
    public void RootAddress_IsStableForHierarchySeed()
    {
        var root = new ArchiveAddress(0, 0, 0, 0, 0, 0);
        var first = ArchiveSeed.DeriveHierarchySeed(root, worldSeed: 46);
        var second = ArchiveSeed.DeriveHierarchySeed(root, worldSeed: 46);

        Assert.Equal(first, second);
    }
}
