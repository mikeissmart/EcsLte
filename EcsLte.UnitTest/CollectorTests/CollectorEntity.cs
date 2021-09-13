using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.CollectorTests
{
    [TestClass]
    public class CollectorEntity : BasePrePostTest
    {
        [TestMethod]
        public void AddedEntity()
        {
            var collector = _world.CollectorManager.GetCollector(CollectorTrigger.Added(Filter.AllOf<TestComponent1>()));
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
            var collector = _world.CollectorManager.GetCollector(CollectorTrigger.Added(Filter.AllOf<TestComponent1>()));
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            collector.ClearEntities();

            // Correct count
            Assert.IsTrue(collector.GetEntities().Length == 0);
        }

        [TestMethod]
        public void RemovedCollectorTriggeredEntity()
        {
            var collector = _world.CollectorManager.GetCollector(CollectorTrigger.Removed(Filter.AllOf<TestComponent1>()));
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            _world.EntityManager.RemoveComponent<TestComponent1>(entity);

            // No longer has entity
            Assert.IsTrue(collector.GetEntities().Length == 1);
            // Correct entity
            Assert.IsTrue(collector.GetEntities()[0] == entity);
        }

        [TestMethod]
        public void RemovedDeletedEntity()
        {
            var collector = _world.CollectorManager.GetCollector(CollectorTrigger.Removed(Filter.AllOf<TestComponent1>()));
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());
            _world.EntityManager.RemoveComponent<TestComponent1>(entity);

            _world.EntityManager.DestroyEntity(entity);

            // No longer has entity
            Assert.IsTrue(collector.GetEntities().Length == 0);
        }
    }
}