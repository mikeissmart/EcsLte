using System;
using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.GroupTests
{
    [TestClass]
    public class GroupLife : BasePrePostTest
    {
        [TestMethod]
        public void GetGroup()
        {
            var _filter = Filter.AllOf<TestComponent1>();

            var group = _world.GroupManager.GetGroup(_filter);

            // Not null
            Assert.IsTrue(group != null);
            // Correct count
            Assert.IsTrue(_world.GroupManager.Groups.Length == 1);
            // Has group
            Assert.IsTrue(_world.GroupManager.Groups[0] == group);
            // Get same group
            Assert.IsTrue(_world.GroupManager.GetGroup(_filter) == group);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.GroupManager.GetGroup(_filter));
        }

        [TestMethod]
        public void RemoveGroup()
        {
            var _filter = Filter.AllOf<TestComponent1>();
            var group = _world.GroupManager.GetGroup(_filter);

            _world.GroupManager.RemoveGroup(group);

            // Correct count
            Assert.IsTrue(_world.GroupManager.Groups.Length == 0);
            // Remove group again
            Assert.ThrowsException<GroupIsDestroyedException>(() =>
                _world.GroupManager.RemoveGroup(group));
            // Remove different world group
            Assert.ThrowsException<GroupDoesNotExistException>(() =>
                _world.GroupManager.RemoveGroup(World.DefaultWorld.GroupManager.GetGroup(_filter)));
            // Error on null
            Assert.ThrowsException<ArgumentNullException>(() =>
                _world.GroupManager.RemoveGroup(null));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.GroupManager.RemoveGroup(group));

            World.DefaultWorld.GroupManager.RemoveGroup(
                World.DefaultWorld.GroupManager.GetGroup(_filter));
        }

        [TestMethod]
        public void InternalDestroy()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

            World.DestroyWorld(_world);

            // Group destroyed
            Assert.IsTrue(group.IsDestroyed);
        }
    }
}