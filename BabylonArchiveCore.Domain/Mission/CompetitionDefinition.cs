namespace BabylonArchiveCore.Domain.Mission;

/// <summary>
/// Competition sub-mode.
/// </summary>
public enum CompetitionMode
{
    /// <summary>First team to reach the objective wins.</summary>
    Race,

    /// <summary>Team that controls more points at time limit wins.</summary>
    Control,
}

/// <summary>
/// Identifies which side a team is on.
/// </summary>
public enum TeamSide
{
    Alpha,
    Beta,
}

/// <summary>
/// A team participating in a Competition mission.
/// </summary>
public sealed class CompetitionTeam
{
    public required TeamSide Side { get; init; }
    public required string Name { get; init; }
    public int Score { get; set; }
}

/// <summary>
/// Definition of a Competition-type mission: two teams race or struggle for control.
/// The outcome determines the page's fate in the Archive.
/// </summary>
public sealed class CompetitionDefinition
{
    public required string MissionId { get; init; }
    public required CompetitionMode Mode { get; init; }
    public required CompetitionTeam Alpha { get; init; }
    public required CompetitionTeam Beta { get; init; }

    /// <summary>Score required to win in Race mode, or time limit ticks in Control mode.</summary>
    public int TargetScore { get; init; } = 10;

    /// <summary>Effect on WorldState depending on which team wins.</summary>
    public MissionEffect? OnAlphaWins { get; init; }
    public MissionEffect? OnBetaWins { get; init; }
}
