using System;
using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.SystemTests
{
    [TestClass]
    public class SystemLife : BasePrePostTest
    {
        [TestMethod]
        public void GetSystems()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();

            var systems = _world.SystemManager.GetSystems();

            // Correct count
            Assert.IsTrue(systems.Length == 1);
            // Correct systems
            Assert.IsTrue(systems[0] == system);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.GetSystems());
        }

        [TestMethod]
        public void GetActiveSystems()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();
            _world.SystemManager.AddSystem<TestSortSystem2>();
            _world.SystemManager.DeactivateSystem<TestSortSystem2>();

            var activeSystems = _world.SystemManager.GetActiveSystems();

            // Correct count
            Assert.IsTrue(activeSystems.Length == 1);
            // Correct systems
            Assert.IsTrue(activeSystems[0] == system);
            Assert.IsTrue(activeSystems[0].IsActive);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.GetActiveSystems());
        }

        [TestMethod]
        public void GetSystemSorters()
        {
            var system1 = _world.SystemManager.AddSystem<TestSortSystem1>();

            var systemSorters = _world.SystemManager.GetSystemSorters();

            // Correct count
            Assert.IsTrue(systemSorters.Length == 1);
            // Correct systems
            Assert.IsTrue(systemSorters[0].System == system1);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.GetSystemSorters());
        }

        [TestMethod]
        public void GetSystem()
        {
            var system1 = _world.SystemManager.AddSystem<TestSortSystem1>();

            var system2 = _world.SystemManager.GetSystem<TestSortSystem1>();

            // Correct count
            Assert.IsTrue(system2 != null);
            // Correct systems
            Assert.IsTrue(system1 == system2);
            // Get not added system
            Assert.ThrowsException<SystemDoesNotSystemException>(() =>
                _world.SystemManager.GetSystem<TestSortSystem2>());
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.GetSystem<TestSortSystem1>());
        }

        [TestMethod]
        public void HasSystem()
        {
            var system1 = _world.SystemManager.AddSystem<TestSortSystem1>();

            // Correct count
            Assert.IsTrue(_world.SystemManager.HasSystem<TestSortSystem1>());
            // World is destroyed
            Assert.IsFalse(_destroyedWorld.SystemManager.HasSystem<TestSortSystem1>());
        }

        [TestMethod]
        public void AutoAddSystems()
        {
            var systems = _world.SystemManager.AutoAddSystems();

            // Correct count
            Assert.IsTrue(_world.SystemManager.GetSystems().Length == 2);
            // Correct systems
            Assert.IsTrue(_world.SystemManager.GetSystems()[0] == systems[0]);
            Assert.IsTrue(_world.SystemManager.GetSystems()[1] == systems[1]);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.AutoAddSystems());
        }

        [TestMethod]
        public void AddSystem()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();

            // Correct systems
            Assert.IsTrue(system != null);
            Assert.IsTrue(_world.SystemManager.GetSystems()[0] == system);
            // Correct count
            Assert.IsTrue(_world.SystemManager.GetSystems().Length == 1);
            // Add again
            Assert.ThrowsException<SystemAlreadyHasSystemException>(() =>
                _world.SystemManager.AddSystem<TestSortSystem1>());
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.AddSystem<TestSortSystem1>());
        }

        [TestMethod]
        public void RemoveSystem()
        {
            _world.SystemManager.AddSystem<TestSortSystem1>();

            _world.SystemManager.RemoveSystem<TestSortSystem1>();

            // Correct count
            Assert.IsTrue(_world.SystemManager.GetSystems().Length == 0);
            // Remove again
            Assert.ThrowsException<SystemDoesNotSystemException>(() =>
                _world.SystemManager.RemoveSystem<TestSortSystem1>());
            // Does not have
            Assert.ThrowsException<SystemDoesNotSystemException>(() =>
                _world.SystemManager.RemoveSystem<TestSortSystem2>());
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.AddSystem<TestSortSystem1>());
        }

        [TestMethod]
        public void ActivateSystem()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();
            _world.SystemManager.DeactivateSystem<TestSortSystem1>();

            _world.SystemManager.ActivateSystem<TestSortSystem1>();

            // Correct
            Assert.IsTrue(system.IsActive);
            // Does not have
            Assert.ThrowsException<SystemDoesNotSystemException>(() =>
                _world.SystemManager.ActivateSystem<TestSortSystem2>());
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.ActivateSystem<TestSortSystem1>());
        }

        [TestMethod]
        public void DeactivateSystem()
        {
            var system = _world.SystemManager.AddSystem<TestSortSystem1>();

            _world.SystemManager.DeactivateSystem<TestSortSystem1>();

            // Correct
            Assert.IsFalse(system.IsActive);
            // Does not have
            Assert.ThrowsException<SystemDoesNotSystemException>(() =>
                _world.SystemManager.DeactivateSystem<TestSortSystem2>());
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.SystemManager.DeactivateSystem<TestSortSystem1>());
        }

        [TestMethod]
        public void SortCorrect()
        {
            var system1 = _world.SystemManager.AddSystem<TestSortSystem1>();
            var system2 = _world.SystemManager.AddSystem<TestSortSystem2>();
            var system3 = _world.SystemManager.AddSystem<TestSortSystem3>();

            var systems = _world.SystemManager.GetSystemSorters();

            // Correct 
            Assert.IsTrue(systems.Length == 3);
            Assert.IsTrue(systems[0].System == system1);
            Assert.IsTrue(systems[1].System == system2);
            Assert.IsTrue(systems[2].System == system3);
        }

        [TestMethod]
        public void SortSelf()
        {
            _world.SystemManager.AddSystem<TestSortSelfSystem1>();

            // Correct 
            Assert.ThrowsException<SystemSortException>(() =>
                _world.SystemManager.GetSystemSorters());
        }

        [TestMethod]
        public void SortLoop()
        {
            _world.SystemManager.AddSystem<TestSortLoopSystem1>();
            _world.SystemManager.AddSystem<TestSortLoopSystem2>();

            // Correct 
            Assert.ThrowsException<SystemSortException>(() =>
                _world.SystemManager.GetSystemSorters());
        }
    }
}