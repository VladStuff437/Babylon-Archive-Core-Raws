using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Tests;

public sealed class ArchiveEntryChainTests
{
    [Fact]
    public void OrderedNodes_HasCanonicalSequence()
    {
        var expected = new[]
        {
            ArchiveEntryChain.ArchiveControl,
            ArchiveEntryChain.ArchiveCorridor,
            ArchiveEntryChain.EntryOctagon,
            ArchiveEntryChain.IndexVestibule,
            ArchiveEntryChain.ResearchRoom01,
            ArchiveEntryChain.StackRingPreview,
        };

        Assert.Equal(expected, ArchiveEntryChain.OrderedNodes);
    }

    [Theory]
    [InlineData(false, false, false, false, false, 1)]
    [InlineData(true, false, false, false, false, 2)]
    [InlineData(true, true, false, false, false, 3)]
    [InlineData(true, true, true, false, false, 4)]
    [InlineData(true, true, true, true, false, 5)]
    [InlineData(true, true, true, true, true, 6)]
    public void GetCompletedSteps_ReturnsExpectedProgress(
        bool hardArchivePartialUnlock,
        bool entryOctagonUnlocked,
        bool indexVestibuleUnlocked,
        bool researchRoomUnlocked,
        bool stackRingPreviewUnlocked,
        int expected)
    {
        var progress = ArchiveEntryChain.GetCompletedSteps(
            hardArchivePartialUnlock,
            entryOctagonUnlocked,
            indexVestibuleUnlocked,
            researchRoomUnlocked,
            stackRingPreviewUnlocked);

        Assert.Equal(expected, progress);
    }

    [Theory]
    [InlineData(false, false, false, false, false, ArchiveEntryChain.ArchiveCorridor)]
    [InlineData(true, false, false, false, false, ArchiveEntryChain.EntryOctagon)]
    [InlineData(true, true, false, false, false, ArchiveEntryChain.IndexVestibule)]
    [InlineData(true, true, true, false, false, ArchiveEntryChain.ResearchRoom01)]
    [InlineData(true, true, true, true, false, ArchiveEntryChain.StackRingPreview)]
    [InlineData(true, true, true, true, true, null)]
    public void GetNextNodeId_ReturnsExpectedNode(
        bool hardArchivePartialUnlock,
        bool entryOctagonUnlocked,
        bool indexVestibuleUnlocked,
        bool researchRoomUnlocked,
        bool stackRingPreviewUnlocked,
        string? expected)
    {
        var nextNode = ArchiveEntryChain.GetNextNodeId(
            hardArchivePartialUnlock,
            entryOctagonUnlocked,
            indexVestibuleUnlocked,
            researchRoomUnlocked,
            stackRingPreviewUnlocked);

        Assert.Equal(expected, nextNode);
    }
}
