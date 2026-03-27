using BabylonArchiveCore.Runtime.Input;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session027CombatSmokeTests
{
    [Fact]
    public void CombatInput_TargetCycle_Respects_MissionTransitionLock()
    {
        var input = new CombatInputHandler();
        input.SetTargets(new[] { "enemy-1", "enemy-2", "enemy-3" });

        Assert.Equal("enemy-1", input.CurrentTarget);
        input.TabTarget();
        Assert.Equal("enemy-2", input.CurrentTarget);

        input.BeginMissionTransitionLock();
        input.TabTarget();
        Assert.Equal("enemy-2", input.CurrentTarget);

        input.EndMissionTransitionLock();
        input.TabTarget();
        Assert.Equal("enemy-3", input.CurrentTarget);
    }
}
