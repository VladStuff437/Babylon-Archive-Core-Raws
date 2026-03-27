using BabylonArchiveCore.Core.Archive;
using Xunit;

namespace BabylonArchiveCore.Tests.Generation;

public class Session045SeedGoldenTests
{
    [Fact]
    public void ArchiveAddress_ParseThenSeed_RemainsStable()
    {
        var canonical = "S4:H5:M6:Sh7:T8:P9";
        var parsed = ArchiveAddress.Parse(canonical);
        var reparsed = ArchiveAddress.Parse(parsed.ToCanonicalString());

        var left = ArchiveSeed.ToSeed(parsed, worldSeed: 45);
        var right = ArchiveSeed.ToSeed(reparsed, worldSeed: 45);

        Assert.Equal(left, right);
    }
}
