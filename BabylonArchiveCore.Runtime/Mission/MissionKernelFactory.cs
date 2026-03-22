using BabylonArchiveCore.Domain.Mission;

namespace BabylonArchiveCore.Runtime.Mission;

public sealed class MissionKernelFactory
{
    private readonly TemplateMissionPageGenerator _generator;

    public MissionKernelFactory(TemplateMissionPageGenerator generator)
    {
        _generator = generator;
    }

    public MissionKernel CreateKernel(ArchivePageDefinition page, int seed)
    {
        var generated = _generator.Generate(new PageGenerationContext
        {
            PageId = page.PageId,
            Seed = seed,
            MainTheme = page.DisplayName,
            MissionType = page.MissionType,
            RequiredFlags = page.RequiredFlags,
        });

        return new MissionKernel
        {
            PageId = page.PageId,
            Title = page.DisplayName,
            MissionType = page.MissionType,
            RuntimeNodes = generated.Nodes,
            SuccessFlags = page.RewardFlags,
            FailureFlags = ["PAGE_FAILED"],
            AllowAbortToHub = true,
            AllowReturnToArchive = true,
        };
    }
}
