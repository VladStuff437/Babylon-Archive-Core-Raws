using BabylonArchiveCore.Content.DataContracts;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Content.Pipeline;

/// <summary>
/// Loads A-0 scene content from JSON files and returns ready-to-use domain models.
/// </summary>
public sealed class A0ContentProvider
{
    private readonly string _contentRoot;

    public A0ContentProvider(string contentRoot)
    {
        _contentRoot = contentRoot;
    }

    public List<HubZone> LoadZones()
    {
        var zoneData = ContentLoader.LoadZones(_contentRoot, "A0_Zones.json");
        var objectData = ContentLoader.LoadObjects(_contentRoot, "A0_Objects.json");
        return A0ContentMapper.MapZones(zoneData, objectData);
    }

    public List<InlineDialogue> LoadDialogues()
    {
        var data = ContentLoader.LoadDialogues(_contentRoot, "A0_Dialogue.json");
        return A0ContentMapper.MapDialogues(data);
    }

    public List<PhaseTrigger> LoadTriggers()
    {
        var data = ContentLoader.LoadTriggers(_contentRoot, "A0_Triggers.json");
        return A0ContentMapper.MapTriggers(data);
    }

    public IReadOnlyDictionary<string, IReadOnlyList<TriggerAction>> LoadObjectInteractionTriggers()
    {
        var data = ContentLoader.LoadTriggers(_contentRoot, "A0_Triggers.json");
        return A0ContentMapper.MapObjectInteractionTriggers(data);
    }

    public List<Objective> LoadObjectives()
    {
        var data = ContentLoader.LoadObjectives(_contentRoot, "A0_Objectives.json");
        return A0ContentMapper.MapObjectives(data);
    }

    public SceneData LoadScene()
    {
        return ContentLoader.LoadScene(_contentRoot, "SCN_A0_INITIATION");
    }
}
