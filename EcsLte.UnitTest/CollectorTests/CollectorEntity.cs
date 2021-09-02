using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.CollectorTests
{
    [TestClass]
    public class CollectorEntity : BasePrePostTest
    {
        [TestMethod]
        public void AddedEntity()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());
            var entity = _world.EntityManager.CreateEntity();

            _world.EntityManager.AddComponent(entity, new TestComponent1());

            // Has entity
            Assert.IsTrue(collector.GetEntities().Length == 1);
            // Correct entity
            Assert.IsTrue(collector.GetEntities()[0] == entity);
        }

        [TestMethod]
        public void ClearEntities()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            collector.ClearEntities();

            // Correct count
            Assert.IsTrue(collector.GetEntities().Length == 0);
        }

        [TestMethod]
        public void AddCollectorTriggeredEntity()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());
            var entity1 = _world.EntityManager.CreateEntity();
            var entity2 = _world.EntityManager.CreateEntity();

            _world.EntityManager.AddComponent(entity1, new TestComponent1());
            _world.EntityManager.AddComponent(entity2, new TestComponent2());

            Assert.IsTrue(group.GetEntities().Length == 1);
            Assert.IsTrue(group.GetEntities()[0] == entity1);
        }

        [TestMethod]
        public void RemovedCollectorTriggeredEntity()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            _world.EntityManager.RemoveComponent<TestComponent1>(entity);

            // No longer has entity
            Assert.IsTrue(group.GetEntities().Length == 0);
        }

        [TestMethod]
        public void RemovedDeletedEntity()
        {
            var group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            Assert.IsTrue(collector.GetEntities().Length == 1);
            Assert.IsTrue(collector.GetEntities()[0] == entity);

            _world.EntityManager.DestroyEntity(entity);

            Assert.IsTrue(collector.GetEntities().Length == 0);
        }
    }
}