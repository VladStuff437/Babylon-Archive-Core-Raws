using BabylonArchiveCore.Core.Missions;

namespace BabylonArchiveCore.Runtime.Missions;

/// <summary>
/// Provides a deterministic fallback mission when mission validation fails.
/// </summary>
public sealed class FallbackMissionProvider
{
    public MissionDefinition CreateFallbackMission(string missionId, IEnumerable<string> reasonCodes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(missionId);
        ArgumentNullException.ThrowIfNull(reasonCodes);

        var reasons = reasonCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(code => code, StringComparer.Ordinal)
            .Take(3)
            .ToArray();

        var titleSuffix = reasons.Length == 0 ? "validation-fallback" : string.Join(",", reasons);

        return new MissionDefinition
        {
            ContractVersion = 40,
            MissionId = $"{missionId}.fallback",
            Title = $"Fallback mission ({titleSuffix})",
            StartNodeId = "fallback-start",
            Nodes = new[]
            {
                new MissionNode
                {
                    NodeId = "fallback-start",
                    Description = "Fallback start",
                    IsTerminal = false,
                    IsCheckpoint = true,
                    Transitions = new[]
                    {
                        new MissionTransition
                        {
                            TargetNodeId = "fallback-end",
                            Priority = 1,
                            IsFallback = true
                        }
                    }
                },
                new MissionNode
                {
                    NodeId = "fallback-end",
                    Description = "Fallback terminal",
                    IsTerminal = true,
                    IsCheckpoint = false,
                    Transitions = Array.Empty<MissionTransition>()
                }
            }
        };
    }
}
