using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Domain.Progression;

namespace BabylonArchiveCore.Runtime.Progression;

/// <summary>
/// Manages XP gain, level-up, stat allocation, and perk unlocking.
/// </summary>
public sealed class ProgressionRuntime
{
    private readonly OperatorProfile _profile;
    private readonly EventBus _eventBus;
    private readonly BalanceCurve _balance;
    private readonly Dictionary<string, PerkDefinition> _perkDefs = new();

    public ProgressionRuntime(OperatorProfile profile, EventBus eventBus, BalanceCurve balance)
    {
        _profile = profile;
        _eventBus = eventBus;
        _balance = balance;
    }

    public OperatorProfile Profile => _profile;

    public void RegisterPerk(PerkDefinition perk) => _perkDefs[perk.Id] = perk;

    /// <summary>
    /// Awards XP and triggers level-ups as thresholds are crossed.
    /// </summary>
    public void AwardXp(int amount)
    {
        if (amount <= 0) return;
        _profile.Experience += amount;

        while (_profile.Experience >= _balance.XpForLevel(_profile.Level + 1))
        {
            var oldLevel = _profile.Level;
            _profile.Level++;
            var pointsGranted = _balance.StatPointsPerLevel;
            _profile.StatPointsAvailable += pointsGranted;

            _eventBus.Publish(new LevelUpEvent
            {
                OldLevel = oldLevel,
                NewLevel = _profile.Level,
                StatPointsGranted = pointsGranted,
            });
        }
    }

    /// <summary>
    /// Allocates a single stat point. Returns true on success.
    /// </summary>
    public bool AllocateStat(StatType stat, int points = 1)
    {
        if (points <= 0 || _profile.StatPointsAvailable < points) return false;
        _profile.AddStat(stat, points);
        _profile.StatPointsAvailable -= points;
        return true;
    }

    /// <summary>
    /// Attempts to unlock a perk. Returns false if requirements are not met.
    /// </summary>
    public bool TryUnlockPerk(string perkId)
    {
        if (_profile.UnlockedPerks.Contains(perkId)) return false;
        if (!_perkDefs.TryGetValue(perkId, out var def)) return false;

        if (_profile.Level < def.RequiredLevel) return false;

        foreach (var req in def.RequiredStats)
        {
            if (_profile.GetStat(req.Key) < req.Value) return false;
        }

        foreach (var prereq in def.PrerequisitePerks)
        {
            if (!_profile.UnlockedPerks.Contains(prereq)) return false;
        }

        _profile.UnlockedPerks.Add(perkId);
        return true;
    }

    /// <summary>
    /// Returns all capabilities the operator has from unlocked perks.
    /// </summary>
    public HashSet<string> GetCapabilities()
    {
        var caps = new HashSet<string>();
        foreach (var perkId in _profile.UnlockedPerks)
        {
            if (_perkDefs.TryGetValue(perkId, out var def))
                caps.Add(def.GrantsCapability);
        }
        return caps;
    }
}
