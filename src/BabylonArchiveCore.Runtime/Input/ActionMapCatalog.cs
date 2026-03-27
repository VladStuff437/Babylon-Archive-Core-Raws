using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Input;

/// <summary>
/// Каталог action map профилей для S019.
/// </summary>
public sealed class ActionMapCatalog
{
    private readonly Dictionary<string, Session019ActionMapContract> profiles = new(StringComparer.OrdinalIgnoreCase);

    public void Register(Session019ActionMapContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);

        profiles[contract.ProfileId] = contract;
    }

    public bool TryResolveAction(string profileId, string inputToken, out string action)
    {
        ArgumentNullException.ThrowIfNull(profileId);
        ArgumentNullException.ThrowIfNull(inputToken);

        action = string.Empty;

        if (!profiles.TryGetValue(profileId, out var profile))
        {
            return false;
        }

        if (profile.Bindings.TryGetValue(inputToken, out var mappedAction))
        {
            action = mappedAction;
            return true;
        }

        if (!string.IsNullOrWhiteSpace(profile.FallbackProfileId)
            && profiles.TryGetValue(profile.FallbackProfileId, out var fallback)
            && fallback.Bindings.TryGetValue(inputToken, out var fallbackAction))
        {
            action = fallbackAction;
            return true;
        }

        return false;
    }
}
