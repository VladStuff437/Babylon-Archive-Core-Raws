using BabylonArchiveCore.Runtime.Combat;
using Xunit;

namespace BabylonArchiveCore.Tests.Gameplay;

public class Session028CombatSmokeTests
{
    [Fact]
    public void AutoAttack_TickFlow_SafeStop_Smoke()
    {
        var autoAttack = new AutoAttackController();
        autoAttack.Start("enemy-1", 2);

        var strikes = 0;
        for (var i = 0; i < 5; i++)
        {
            if (autoAttack.Tick())
            {
                strikes++;
            }
        }

        Assert.Equal(3, strikes);
        autoAttack.SafeStop();
        Assert.False(autoAttack.IsActive);
        Assert.Null(autoAttack.CurrentTargetId);
    }
}
