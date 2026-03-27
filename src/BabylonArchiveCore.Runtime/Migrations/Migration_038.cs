using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S038: контракт MissionNode.
/// </summary>
public sealed class Migration_038
{
    public Session038MissionNodeContract Migrate(object? legacyState)
    {
        if (legacyState is Session038MissionNodeContract existing)
        {
            return existing;
        }

        return new Session038MissionNodeContract
        {
            NodeId = "start",
            IsTerminal = false,
            IsCheckpoint = true,
            Transitions = Array.Empty<TransitionContract>()
        };
    }
}
