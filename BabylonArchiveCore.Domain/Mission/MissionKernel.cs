namespace BabylonArchiveCore.Domain.Mission;

public sealed class MissionKernel
{
    public string PageId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string MissionType { get; init; } = string.Empty;
    public IReadOnlyList<string> RuntimeNodes { get; init; } = [];
    public IReadOnlyList<string> SuccessFlags { get; init; } = [];
    public IReadOnlyList<string> FailureFlags { get; init; } = [];
    public bool AllowAbortToHub { get; init; }
    public bool AllowReturnToArchive { get; init; }
}
