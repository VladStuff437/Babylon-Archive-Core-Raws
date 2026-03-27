using BabylonArchiveCore.Core.State;

namespace BabylonArchiveCore.Runtime.State;

/// <summary>
/// Runtime API для мутаций WorldState.
/// </summary>
public sealed class WorldStateService
{
    private readonly WorldState worldState;

    public WorldStateService(WorldState worldState)
    {
        ArgumentNullException.ThrowIfNull(worldState);
        this.worldState = worldState;
    }

    public int GetFactionReputation(string factionId) => worldState.GetFactionReputation(factionId);

    public void SetFactionReputation(string factionId, int value) => worldState.SetFactionReputation(factionId, value);

    public int ChangeFactionReputation(string factionId, int delta) => worldState.ChangeFactionReputation(factionId, delta);

    public void SetAxes(float moralAxis, float technoArcaneAxis) => worldState.SetAxes(moralAxis, technoArcaneAxis);

    public void ApplyAxisDelta(float moralDelta, float technoArcaneDelta) =>
        worldState.ApplyAxisDelta(moralDelta, technoArcaneDelta);

    public (float MoralAxis, float TechnoArcaneAxis) GetAxisSnapshot() => worldState.GetAxisSnapshot();

    public void ApplyMissionEffect(MissionEffect effect) => worldState.ApplyMissionEffect(effect);
}
