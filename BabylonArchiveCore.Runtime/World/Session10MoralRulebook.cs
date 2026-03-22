using BabylonArchiveCore.Domain.World.Morality;

namespace BabylonArchiveCore.Runtime.World;

public static class Session10MoralRulebook
{
    public static IReadOnlyDictionary<string, MoralRule> CreateDefault()
    {
        return new Dictionary<string, MoralRule>(StringComparer.OrdinalIgnoreCase)
        {
            ["EVT_A0_CORE_ACTIVATED"] = new MoralRule
            {
                EventId = "EVT_A0_CORE_ACTIVATED",
                Delta = new MoralDelta(1, 1, 2, 0, 3, 0),
                Tags = ["progression", "responsibility"],
            },
            ["EVT_A0_RESEARCH_ANOMALY_SOLVED"] = new MoralRule
            {
                EventId = "EVT_A0_RESEARCH_ANOMALY_SOLVED",
                Delta = new MoralDelta(0, 2, 2, 0, 2, 3),
                Tags = ["insight", "verification"],
            },
            ["EVT_A0_GATE_FORCED_ATTEMPT"] = new MoralRule
            {
                EventId = "EVT_A0_GATE_FORCED_ATTEMPT",
                Delta = new MoralDelta(-1, -1, -2, -1, -2, 0),
                Tags = ["warning", "impulsive"],
            },
            ["EVT_A0_KNOWLEDGE_CHECK_EXCELLENT"] = new MoralRule
            {
                EventId = "EVT_A0_KNOWLEDGE_CHECK_EXCELLENT",
                Delta = new MoralDelta(0, 2, 2, 1, 1, 4),
                Tags = ["knowledge", "method"],
            },
            ["EVT_A0_PROTOCOL_ZERO_COMPLETED"] = new MoralRule
            {
                EventId = "EVT_A0_PROTOCOL_ZERO_COMPLETED",
                Delta = new MoralDelta(2, 1, 2, 1, 2, 1),
                Tags = ["ceremony", "stability"],
            },
        };
    }
}
