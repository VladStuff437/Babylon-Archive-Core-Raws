using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Domain.World;

namespace BabylonArchiveCore.Runtime.Mission;

/// <summary>
/// Applies <see cref="MissionEffect"/> consequences to <see cref="WorldState"/>.
/// </summary>
public static class WorldStateEffectApplier
{
    public static void Apply(WorldState world, MissionEffect effect, ILogger logger)
    {
        if (effect.SetFlag is not null)
        {
            world.SetFlag(effect.SetFlag);
            logger.Info($"WorldEffect: flag '{effect.SetFlag}' set.");
        }

        if (effect.MoralDelta != 0)
        {
            world.AdjustMoral(effect.MoralDelta);
            logger.Info($"WorldEffect: moral {effect.MoralDelta:+0;-0} → {world.MoralAxis}");
        }

        if (effect.TechnoArcaneDelta != 0)
        {
            world.AdjustTechnoArcane(effect.TechnoArcaneDelta);
            logger.Info($"WorldEffect: techno-arcane {effect.TechnoArcaneDelta:+0;-0} → {world.TechnoArcaneAxis}");
        }

        foreach (var (entity, delta) in effect.RelationDeltas)
        {
            world.AdjustRelation(entity, delta);
            logger.Info($"WorldEffect: relation '{entity}' {delta:+0;-0} → {world.EntityRelations[entity]}");
        }
    }
}
