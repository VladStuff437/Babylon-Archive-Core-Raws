using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Domain.World.Runtime;

namespace BabylonArchiveCore.Runtime.Mission;

public sealed class MissionLaunchValidator
{
    public MissionLaunchValidationResult Validate(
        ArchivePageDefinition page,
        WorldRuntimeProfile profile,
        IReadOnlyCollection<string> worldFlags)
    {
        if (profile.EnableMissionPageDebugAccess)
        {
            return new MissionLaunchValidationResult
            {
                CanLaunch = true,
                IsAdminOverride = true,
                LaunchMode = "AdminPreview",
            };
        }

        var unmet = page.RequiredFlags.Any(flag => !worldFlags.Contains(flag));
        if (unmet)
        {
            return new MissionLaunchValidationResult
            {
                CanLaunch = false,
                BlockReason = "Не выполнены условия страницы.",
                LaunchMode = "Blocked",
            };
        }

        if (!page.IsUnlockedInPlayerMode)
        {
            return new MissionLaunchValidationResult
            {
                CanLaunch = false,
                BlockReason = "Страница пока недоступна.",
                LaunchMode = "Blocked",
            };
        }

        return new MissionLaunchValidationResult
        {
            CanLaunch = true,
            LaunchMode = "Player",
        };
    }
}
