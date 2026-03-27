using BabylonArchiveCore.Core.Missions;

namespace BabylonArchiveCore.Runtime.Missions.Validation;

public sealed class MissionValidationIssue
{
    public required string Code { get; init; }

    public required string Message { get; init; }

    public string? NodeId { get; init; }
}

public sealed class MissionValidationResult
{
    public required IReadOnlyList<MissionValidationIssue> Issues { get; init; }

    public bool IsValid => Issues.Count == 0;
}

public interface IMissionValidator
{
    MissionValidationResult Validate(MissionDefinition definition);
}

public sealed class MissionValidationPipeline
{
    private readonly IReadOnlyList<IMissionValidator> validators;

    public MissionValidationPipeline(params IMissionValidator[] validators)
    {
        ArgumentNullException.ThrowIfNull(validators);
        this.validators = validators;
    }

    public MissionValidationResult Validate(MissionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var issues = new List<MissionValidationIssue>();
        foreach (var validator in validators)
        {
            var result = validator.Validate(definition);
            issues.AddRange(result.Issues);
        }

        return new MissionValidationResult
        {
            Issues = issues
        };
    }
}
