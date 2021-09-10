using System;
using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.SystemTests
{
    [TestClass]
    public class SystemExecution : BasePrePostTest
    {
        [TestMethod]
        public void InitializeSystems()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();

            _world.SystemManager.InitializeSystems();

            // Correct count
            Assert.IsTrue(system.InitializeCalled == 1);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.InitializeSystems());
        }

        [TestMethod]
        public void ExecuteSystems()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();

            _world.SystemManager.ExecuteSystems();

            // Correct count
            Assert.IsTrue(system.ExecuteCalled == 1);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.ExecuteSystems());
        }

        [TestMethod]
        public void CleanupSystems()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();

            _world.SystemManager.CleanupSystems();

            // Correct count
            Assert.IsTrue(system.CleanupCalled == 1);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.CleanupSystems());
        }

        [TestMethod]
        public void TearDownSystems()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();

            _world.SystemManager.TearDownSystems();

            // Correct count
            Assert.IsTrue(system.TearDownCalled == 1);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.TearDownSystems());
        }

        [TestMethod]
        public void ConstructSystems()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();

            // Correct count
            Assert.IsTrue(system.CostructorCalled);
        }
    }
}