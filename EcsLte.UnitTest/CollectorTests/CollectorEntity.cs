using System.Threading;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.CollectorTests
{
    [TestClass]
    public class CollectorEntity
    {
        [TestInitialize]
        public void PreTest()
        {
            if (!World.DefaultWorld.IsDestroyed)
                World.DestroyWorld(World.DefaultWorld);
            World.DefaultWorld = World.CreateWorld("DefaultWorld");
        }

        [TestMethod]
        public void AddEntity()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1());

            Assert.IsTrue(collector.Entities.Length == 1);
            Assert.IsTrue(collector.Entities[0] == entity);
        }

        [TestMethod]
        public void AddCollectorTriggeredEntity()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());
            var entity1 = world.EntityManager.CreateEntity();
            var entity2 = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity1, new TestComponent1());
            world.EntityManager.AddComponent(entity2, new TestComponent2());

            Assert.IsTrue(group.Entities.Length == 1);
            Assert.IsTrue(group.Entities[0] != entity2);
        }
    }
}