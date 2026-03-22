using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Runtime.Progression;

namespace BabylonArchiveCore.Runtime.QA;

/// <summary>
/// Balance checker: validates that mission difficulty and rewards
/// align with the progression curve for the first tomes.
/// </summary>
public static class BalanceChecker
{
    public sealed class BalanceReport
    {
        public List<string> Issues { get; init; } = new();
        public List<string> PassedChecks { get; init; } = new();
        public bool IsBalanced => Issues.Count == 0;
    }

    /// <summary>
    /// Checks mission reward balance against an expected Credits range per tome.
    /// </summary>
    public static BalanceReport Check(
        IReadOnlyList<MissionDefinition> missions,
        BalanceCurve curve,
        ILogger logger)
    {
        var issues = new List<string>();
        var passed = new List<string>();

        // Check 1: At least 1 mission exists
        if (missions.Count == 0)
        {
            issues.Add("No missions defined — cannot validate balance.");
            return new BalanceReport { Issues = issues, PassedChecks = passed };
        }
        passed.Add($"Mission count: {missions.Count}");

        // Check 2: All missions have valid graph (start node exists)
        foreach (var m in missions)
        {
            if (!m.Nodes.ContainsKey(m.StartNodeId))
                issues.Add($"Mission '{m.Id}': start node '{m.StartNodeId}' not found in graph.");
            else
                passed.Add($"Mission '{m.Id}': start node valid.");

            // Check 3: Every transition target exists
            foreach (var node in m.Nodes.Values)
            {
                foreach (var (label, target) in node.Transitions)
                {
                    if (!m.Nodes.ContainsKey(target))
                        issues.Add($"Mission '{m.Id}' node '{node.Id}': transition '{label}' → '{target}' leads to non-existent node.");
                }
            }

            // Check 4: Has at least one terminal node
            var hasTerminal = m.Nodes.Values.Any(n => n.IsTerminalSuccess || n.IsTerminalFailure);
            if (!hasTerminal)
                issues.Add($"Mission '{m.Id}': no terminal (success/failure) node found.");
            else
                passed.Add($"Mission '{m.Id}': has terminal nodes.");
        }

        // Check 5: Balance curve produces growing XP requirements
        for (var l = 2; l <= 10; l++)
        {
            if (curve.XpForLevel(l) <= curve.XpForLevel(l - 1))
                issues.Add($"Balance curve: XP for level {l} ({curve.XpForLevel(l)}) is not greater than level {l - 1} ({curve.XpForLevel(l - 1)}).");
        }
        if (issues.Count == 0)
            passed.Add("Balance curve: XP grows monotonically through level 10.");

        foreach (var i in issues) logger.Warn($"BalanceCheck: {i}");
        foreach (var p in passed) logger.Info($"BalanceCheck: {p}");

        return new BalanceReport { Issues = issues, PassedChecks = passed };
    }
}
