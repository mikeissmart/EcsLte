using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.GroupTests
{
    [TestClass]
    public class GroupEntity : BasePrePostTest
    {
        [TestMethod]
        public void FilteredEntity()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var entity1 = _world.EntityManager.CreateEntity();
            var entity2 = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity1, new TestComponent1());

            // Has entity
            Assert.IsTrue(group.GetEntities().Length == 1);
            // Correct entity
            Assert.IsTrue(group.GetEntities()[0] == entity1);
            // Group is destroyed
            _world.GroupManager.RemoveGroup(group);
            Assert.ThrowsException<GroupIsDestroyedException>(() =>
                group.GetEntities());
        }

        [TestMethod]
        public void RemovedUnfilteredEntity()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            _world.EntityManager.RemoveComponent<TestComponent1>(entity);

            // No longer has entity
            Assert.IsTrue(group.GetEntities().Length == 0);
        }

        [TestMethod]
        public void KeepUpdatedEntity()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            _world.EntityManager.ReplaceComponent(entity, new TestComponent1());

            // No longer has entity
            Assert.IsTrue(group.GetEntities().Length == 1);
            // Correct entity
            Assert.IsTrue(group.GetEntities()[0] == entity);
        }

        [TestMethod]
        public void RemovedDeletedEntity()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            _world.EntityManager.DestroyEntity(entity);

            // No longer has entity
            Assert.IsTrue(group.GetEntities().Length == 0);
        }
    }
}