using BabylonArchiveCore.Domain.Mission;

namespace BabylonArchiveCore.Runtime.Mission;

public sealed record OperationLaunchDecision(bool Allowed, string Reason);

public sealed record TerminalOperationCard(
    string MissionId,
    string Title,
    MissionType Type,
    MissionTerminalTab Tab,
    bool RequiresProtocolZero,
    bool IsReplayable);

/// <summary>
/// Session 8.537 bridge: terminal operation listing and launch eligibility rules.
/// </summary>
public static class OperationLaunchGate
{
    public static OperationLaunchDecision Evaluate(
        MissionDefinition mission,
        bool protocolZeroUnlocked,
        MissionStatus currentStatus)
    {
        if (mission.RequiresProtocolZero && !protocolZeroUnlocked)
        {
            return new(false, "Protocol Zero is required.");
        }

        if (currentStatus == MissionStatus.Active)
        {
            return new(false, "Mission is already active.");
        }

        if (currentStatus == MissionStatus.Completed && !mission.IsReplayable)
        {
            return new(false, "Mission cannot be replayed.");
        }

        return new(true, "OK");
    }

    public static IReadOnlyDictionary<MissionTerminalTab, IReadOnlyList<TerminalOperationCard>> BuildCatalog(
        IEnumerable<MissionDefinition> missions)
    {
        var grouped = missions
            .Select(m => new TerminalOperationCard(
                m.Id,
                m.Title,
                m.Type,
                m.TerminalTab,
                m.RequiresProtocolZero,
                m.IsReplayable))
            .GroupBy(m => m.Tab)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<TerminalOperationCard>)g.OrderBy(x => x.Title).ToList());

        // Always expose all tabs to keep terminal UI stable.
        foreach (var tab in Enum.GetValues<MissionTerminalTab>())
        {
            if (!grouped.ContainsKey(tab))
            {
                grouped[tab] = [];
            }
        }

        return grouped;
    }
}
