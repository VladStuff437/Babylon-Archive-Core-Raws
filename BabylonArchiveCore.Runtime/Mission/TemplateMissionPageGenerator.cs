using BabylonArchiveCore.Domain.Mission;

namespace BabylonArchiveCore.Runtime.Mission;

public sealed class TemplateMissionPageGenerator
{
    public GeneratedMissionPage Generate(PageGenerationContext context)
    {
        string[] nodes = context.MissionType switch
        {
            "Investigation" =>
            [
                "Entry",
                "Orientation",
                "SignalTrace",
                "VerifyEcho",
                "Decision",
                "Resolution",
                "ArchiveUpdate",
            ],
            "ResearchPuzzle" =>
            [
                "Entry",
                "IndexRead",
                "CrossCheck",
                "ConflictNode",
                "Interpretation",
                "Resolution",
                "ArchiveUpdate",
            ],
            _ =>
            [
                "Entry",
                "Discovery",
                "Verification",
                "Decision",
                "Resolution",
                "ArchiveUpdate",
            ],
        };

        return new GeneratedMissionPage
        {
            PageId = context.PageId,
            Title = $"{context.MainTheme} // {context.PageId}",
            Nodes = nodes,
            Rewards = ["XP_SMALL", "FRAGMENT_CHANCE", "JOURNAL_ENTRY"],
            Consequences = ["WORLD_FLAG", "ARCHIVE_TRACE"],
        };
    }
}
