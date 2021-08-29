using System.Threading;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.GroupTests
{
    [TestClass]
    public class GroupEntity
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
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1());

            Assert.IsTrue(group.Entities.Length == 1);
            Assert.IsTrue(group.Entities[0] == entity);
        }

        [TestMethod]
        public void AddFilteredEntity()
        {
            var world = World.DefaultWorld;
            var group = world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            var entity1 = world.EntityManager.CreateEntity();
            var entity2 = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity1, new TestComponent1());
            world.EntityManager.AddComponent(entity2, new TestComponent2());

            Assert.IsTrue(group.Entities.Length == 1);
            Assert.IsTrue(group.Entities[0] != entity2);
        }
    }
}