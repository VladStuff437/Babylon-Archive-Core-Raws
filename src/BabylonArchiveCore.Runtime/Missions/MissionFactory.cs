using BabylonArchiveCore.Core.Missions;
using BabylonArchiveCore.Runtime.Balance;
using BabylonArchiveCore.Runtime.Missions.Validation;

namespace BabylonArchiveCore.Runtime.Missions;

/// <summary>
/// Mission creation pipeline with validation and fallback handling.
/// </summary>
public sealed class MissionFactory
{
    private readonly MissionValidationPipeline validationPipeline;
    private readonly FallbackMissionProvider fallbackMissionProvider;
    private readonly MissionRuntimeEngine runtimeEngine = new();

    public MissionFactory(
        MissionValidationPipeline? validationPipeline = null,
        FallbackMissionProvider? fallbackMissionProvider = null)
    {
        this.validationPipeline = validationPipeline ?? new MissionValidationPipeline(
            new ReachabilityValidator(),
            new DeadEndValidator(),
            new CycleSafetyValidator());

        this.fallbackMissionProvider = fallbackMissionProvider ?? new FallbackMissionProvider();
    }

    public MissionFactoryResult Create(MissionDefinition definition, BalanceTable? balanceTable = null)
    {
        ArgumentNullException.ThrowIfNull(definition);

        _ = balanceTable;

        var issues = new List<MissionValidationIssue>();
        issues.AddRange(definition.Validate().Select(error => new MissionValidationIssue
        {
            Code = "MVAL-STRUCTURE",
            Message = error,
            NodeId = definition.StartNodeId
        }));

        var pipelineResult = validationPipeline.Validate(definition);
        issues.AddRange(pipelineResult.Issues);

        var usedFallback = issues.Count > 0;
        var effectiveDefinition = usedFallback
            ? fallbackMissionProvider.CreateFallbackMission(definition.MissionId, issues.Select(i => i.Code))
            : definition;

        var state = runtimeEngine.Start(effectiveDefinition);
        return new MissionFactoryResult
        {
            Definition = effectiveDefinition,
            RuntimeState = state,
            ValidationIssues = issues,
            UsedFallback = usedFallback
        };
    }
}

public sealed class MissionFactoryResult
{
    public required MissionDefinition Definition { get; init; }

    public required MissionRuntimeState RuntimeState { get; init; }

    public required IReadOnlyList<MissionValidationIssue> ValidationIssues { get; init; }

    public bool UsedFallback { get; init; }
}
