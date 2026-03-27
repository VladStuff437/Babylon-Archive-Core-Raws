using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S041: reachability contract.
/// </summary>
public sealed class Migration_041
{
    public Session041ReachabilityContract Migrate(object? legacyState)
    {
        if (legacyState is Session041ReachabilityContract existing)
        {
            return existing;
        }

        return new Session041ReachabilityContract
        {
            ContractVersion = 41,
            MissionId = "mission-041",
            StartNodeId = "start",
            NodeIds = new[] { "start" }
        };
    }
}
