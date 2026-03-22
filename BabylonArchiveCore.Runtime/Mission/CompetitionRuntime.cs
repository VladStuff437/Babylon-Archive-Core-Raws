using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Mission;

namespace BabylonArchiveCore.Runtime.Mission;

/// <summary>
/// Executes a Competition: two teams accumulate score.
/// Race mode: first to TargetScore wins.
/// Control mode: after TargetScore ticks, highest score wins.
/// </summary>
public sealed class CompetitionRuntime
{
    private readonly CompetitionDefinition _def;
    private readonly ILogger _logger;

    public CompetitionRuntime(CompetitionDefinition definition, ILogger logger)
    {
        _def = definition;
        _logger = logger;
    }

    public TeamSide? Winner { get; private set; }
    public bool IsFinished => Winner is not null;

    /// <summary>
    /// Add score to a team. In Race mode this may immediately finish.
    /// </summary>
    public void AddScore(TeamSide side, int points)
    {
        if (IsFinished) return;

        var team = side == TeamSide.Alpha ? _def.Alpha : _def.Beta;
        team.Score += points;
        _logger.Info($"Competition '{_def.MissionId}': {side} +{points} → {team.Score}");

        if (_def.Mode == CompetitionMode.Race && team.Score >= _def.TargetScore)
        {
            Winner = side;
            _logger.Info($"Competition '{_def.MissionId}': {side} wins by race ({team.Score} >= {_def.TargetScore}).");
        }
    }

    /// <summary>
    /// For Control mode: call after TargetScore ticks to resolve the winner.
    /// </summary>
    public void ResolveControl()
    {
        if (IsFinished) return;
        if (_def.Mode != CompetitionMode.Control) return;

        Winner = _def.Alpha.Score >= _def.Beta.Score ? TeamSide.Alpha : TeamSide.Beta;
        _logger.Info($"Competition '{_def.MissionId}': control resolved — {Winner} wins " +
                     $"(Alpha={_def.Alpha.Score}, Beta={_def.Beta.Score}).");
    }

    /// <summary>
    /// Returns the WorldState effect for the winning team, or null.
    /// </summary>
    public MissionEffect? GetWinnerEffect() => Winner switch
    {
        TeamSide.Alpha => _def.OnAlphaWins,
        TeamSide.Beta => _def.OnBetaWins,
        _ => null,
    };
}
