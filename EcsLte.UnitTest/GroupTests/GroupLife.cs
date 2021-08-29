using System.Threading;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.GroupTests
{
    [TestClass]
    public class GroupLife
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

            Assert.IsTrue(group != null);
        }

        [TestMethod]
        public void GetSame()
        {
            var world = World.DefaultWorld;
            var group1 = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var group2 = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

            Assert.IsTrue(group1 == group2);
        }

        [TestMethod]
        public void SelfDestroy()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());

            world.GroupManager.RemoveGroup(group);

            Assert.IsTrue(group.IsDestroyed);
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