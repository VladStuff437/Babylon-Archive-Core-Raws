namespace BabylonArchiveCore.Runtime.Input;

/// <summary>
/// Консолидация active/fallback профилей управления.
/// </summary>
public sealed class ControlProfileResolver
{
    public string Resolve(string activeProfileId, string fallbackProfileId, IReadOnlyCollection<string> availableProfiles, bool strictResolution)
    {
        ArgumentNullException.ThrowIfNull(activeProfileId);
        ArgumentNullException.ThrowIfNull(fallbackProfileId);
        ArgumentNullException.ThrowIfNull(availableProfiles);

        if (ContainsProfile(availableProfiles, activeProfileId))
        {
            return activeProfileId;
        }

        if (ContainsProfile(availableProfiles, fallbackProfileId))
        {
            return fallbackProfileId;
        }

        if (strictResolution)
        {
            throw new InvalidOperationException("Neither active nor fallback control profile is available.");
        }

        return availableProfiles.FirstOrDefault() ?? fallbackProfileId;
    }

    private static bool ContainsProfile(IReadOnlyCollection<string> availableProfiles, string profileId)
    {
        return availableProfiles.Any(profile => string.Equals(profile, profileId, StringComparison.OrdinalIgnoreCase));
    }
}
