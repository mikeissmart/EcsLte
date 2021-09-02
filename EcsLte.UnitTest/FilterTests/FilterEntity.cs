using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.FilterTests
{
    [TestClass]
    public class FilterEntity
    {
        [TestInitialize]
        public void PreTest()
        {
            if (!World.DefaultWorld.IsDestroyed)
                World.DestroyWorld(World.DefaultWorld);
            World.DefaultWorld = World.CreateWorld("Default");
        }

        [TestMethod]
        public void AllOf()
        {
            var world = World.DefaultWorld;
            var filter = Filter.AllOf<TestComponent1>();
            var entity1 = world.EntityManager.CreateEntity();
            var entity2 = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity1, new TestComponent1());
            world.EntityManager.AddComponent(entity2, new TestComponent2());

            Assert.IsTrue(world.EntityManager.EntityIsFiltered(entity1, filter));
            Assert.IsFalse(world.EntityManager.EntityIsFiltered(entity2, filter));
        }

        [TestMethod]
        public void AnyOf()
        {
            var world = World.DefaultWorld;
            var filter = Filter.AnyOf<TestComponent1>();
            var entity1 = world.EntityManager.CreateEntity();
            var entity2 = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity1, new TestComponent1());
            world.EntityManager.AddComponent(entity2, new TestComponent2());

            Assert.IsTrue(world.EntityManager.EntityIsFiltered(entity1, filter));
            Assert.IsFalse(world.EntityManager.EntityIsFiltered(entity2, filter));
        }

        [TestMethod]
        public void NoneOf()
        {
            var world = World.DefaultWorld;
            var filter = Filter.NoneOf<TestComponent1>();
            var entity1 = world.EntityManager.CreateEntity();
            var entity2 = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity1, new TestComponent1());
            world.EntityManager.AddComponent(entity2, new TestComponent2());

            Assert.IsFalse(world.EntityManager.EntityIsFiltered(entity1, filter));
            Assert.IsTrue(world.EntityManager.EntityIsFiltered(entity2, filter));
        }
    }
}