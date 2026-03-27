using System;
using BabylonArchiveCore.Runtime.State;
using Xunit;

namespace BabylonArchiveCore.Tests.Smoke
{
    public class Session021CombatSmokeTests
    {
        [Fact]
        public void CameraStateManager_SupportsAllModes()
        {
            var manager = new CameraStateManager();
            manager.SetMode("Follow");
            Assert.Equal("Follow", manager.GetState().Mode);
            manager.SetMode("Aim");
            Assert.Equal("Aim", manager.GetState().Mode);
            manager.SetMode("Inspect");
            Assert.Equal("Inspect", manager.GetState().Mode);
        }
    }
}
