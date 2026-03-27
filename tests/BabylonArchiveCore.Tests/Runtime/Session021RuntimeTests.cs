using System;
using BabylonArchiveCore.Runtime.State;
using Xunit;

namespace BabylonArchiveCore.Tests.Runtime
{
    public class Session021RuntimeTests
    {
        [Fact]
        public void CameraStateManager_InitializesWithDefaultMode()
        {
            var manager = new CameraStateManager();
            var state = manager.GetState();
            Assert.Equal("Follow", state.Mode);
        }

        [Fact]
        public void CameraStateManager_CanSwitchModes()
        {
            var manager = new CameraStateManager();
            manager.SetMode("Aim");
            Assert.Equal("Aim", manager.GetState().Mode);
            manager.SetMode("Inspect");
            Assert.Equal("Inspect", manager.GetState().Mode);
        }

        [Fact]
        public void CameraStateManager_ThrowsOnInvalidMode()
        {
            var manager = new CameraStateManager();
            Assert.Throws<ArgumentException>(() => manager.SetMode("Invalid"));
        }

        [Fact]
        public void CameraStateManager_UpdatesPositionAndTarget()
        {
            var manager = new CameraStateManager();
            manager.UpdatePosition(1,2,3);
            manager.UpdateTarget(4,5,6);
            var state = manager.GetState();
            Assert.Equal(1, state.PositionX);
            Assert.Equal(2, state.PositionY);
            Assert.Equal(3, state.PositionZ);
            Assert.Equal(4, state.TargetX);
            Assert.Equal(5, state.TargetY);
            Assert.Equal(6, state.TargetZ);
        }
    }
}
