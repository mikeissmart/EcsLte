using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.CollectorTests
{
    [TestClass]
    public class CollectorLife : BasePrePostTest
    {
        [TestMethod]
        public void Create()
        {
            var collector = _world.CollectorManager.GetCollector(CollectorTrigger.Added(Filter.AllOf<TestComponent1>()));

            Assert.IsTrue(collector != null);
        }

        /*[TestMethod] // TODO
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
        }*/
    }
}