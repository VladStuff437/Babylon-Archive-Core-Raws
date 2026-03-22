using BabylonArchiveCore.Domain.Events.Semantic;
using BabylonArchiveCore.Domain.World;
using BabylonArchiveCore.Domain.World.Morality;

namespace BabylonArchiveCore.Runtime.World;

public sealed class MoralService
{
    private readonly IReadOnlyDictionary<string, MoralRule> _rulebook;

    public MoralService(IReadOnlyDictionary<string, MoralRule> rulebook)
    {
        _rulebook = rulebook;
    }

    public bool TryApply(SemanticEventRecord semanticEvent, WorldState world, IList<MoralLogEntry> log)
    {
        if (!_rulebook.TryGetValue(semanticEvent.EventId, out var rule))
            return false;

        world.ApplyMoralDelta(rule.Delta);

        log.Add(new MoralLogEntry
        {
            EventId = semanticEvent.EventId,
            SourceObjectId = semanticEvent.SourceObjectId,
            Delta = rule.Delta,
            At = semanticEvent.RaisedAt,
            Tags = rule.Tags,
        });

        return true;
    }
}
