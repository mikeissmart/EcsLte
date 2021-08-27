using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandPlayback
{
    [TestClass]
    public class EntityCommandEntityComponent
    {
        [TestInitialize]
        public void PreTest()
        {
            if (!World.DefaultWorld.IsDestroyed)
                World.DestroyWorld(World.DefaultWorld);
            World.DefaultWorld = World.CreateWorld("DefaultWorld");
        }

        [TestMethod]
        public void AddComponent()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.DefaultEntityCommandPlayback.AddComponent(entity, new TestComponent1());
            Assert.IsFalse(world.EntityManager.HasComponent<TestComponent1>(entity));

            world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsTrue(world.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponent()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1());
            world.EntityManager.DefaultEntityCommandPlayback.RemoveComponent<TestComponent1>(entity);
            Assert.IsTrue(world.EntityManager.HasComponent<TestComponent1>(entity));

            world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsFalse(world.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void ReplaceComponent()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1());
            world.EntityManager.DefaultEntityCommandPlayback.ReplaceComponent(entity, new TestComponent1 { Prop = 1 });
            Assert.IsTrue(world.EntityManager.GetComponent<TestComponent1>(entity).Prop == 0);

            world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsTrue(world.EntityManager.GetComponent<TestComponent1>(entity).Prop == 1);
        }
    }
}