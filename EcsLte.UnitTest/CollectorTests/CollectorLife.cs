using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.CollectorTests
{
    [TestClass]
    public class CollectorLife
    {
        [TestInitialize]
        public void PreTest()
        {
            if (!World.DefaultWorld.IsDestroyed)
                World.DestroyWorld(World.DefaultWorld);
            World.DefaultWorld = World.CreateWorld("DefaultWorld");
        }

        [TestMethod]
        public void Create()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());

            Assert.IsTrue(collector != null);
        }

        [TestMethod]
        public void CreateWrongComponent()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

            Assert.ThrowsException<CollectorGroupMissingComponent>(() =>
                group.GetCollector(CollectorTrigger.Added<TestComponent2>()));
        }

        [TestMethod]
        public void GetSame()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector1 = group.GetCollector(CollectorTrigger.Added<TestComponent1>());
            var collector2 = group.GetCollector(CollectorTrigger.Added<TestComponent1>());

            Assert.IsTrue(collector1 == collector2);
        }

        [TestMethod]
        public void SelfDestroy()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());

            group.RemoveCollector(collector);

            Assert.IsTrue(collector.IsDestroyed);
        }

        [TestMethod]
        public void GroupDestroy()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());

            world.GroupManager.RemoveGroup(group);

            Assert.IsTrue(collector.IsDestroyed);
        }

        [TestMethod]
        public void WorldDestroy()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var collector = group.GetCollector(CollectorTrigger.Added<TestComponent1>());

            World.DestroyWorld(world);

            Assert.IsTrue(collector.IsDestroyed);
        }
    }
}