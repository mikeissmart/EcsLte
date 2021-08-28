using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManangerTests
{
    [TestClass]
    public class EntityComponent
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

            world.EntityManager.AddComponent(entity, new TestComponent1());

            Assert.IsTrue(world.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void AddComponentAlready()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1());

            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                world.EntityManager.AddComponent(entity, new TestComponent1()));
        }

        [TestMethod]
        public void GetComponent()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1 { Prop = 1 });

            Assert.IsTrue(world.EntityManager.GetComponent<TestComponent1>(entity).Prop == 1);
        }

        [TestMethod]
        public void GetComponentNotAdded()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                world.EntityManager.GetComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponent()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1());
            world.EntityManager.RemoveComponent<TestComponent1>(entity);

            Assert.IsFalse(world.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void RemoveAllComponents()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1());
            world.EntityManager.AddComponent(entity, new TestComponent2());
            world.EntityManager.RemoveAllComponents(entity);

            Assert.IsFalse(world.EntityManager.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(world.EntityManager.HasComponent<TestComponent2>(entity));
        }

        [TestMethod]
        public void RemoveComponentNotAdded()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                world.EntityManager.RemoveComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void ReplaceComponent()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponent(entity, new TestComponent1());
            world.EntityManager.ReplaceComponent(entity, new TestComponent1 { Prop = 1 });

            Assert.IsTrue(world.EntityManager.GetComponent<TestComponent1>(entity).Prop == 1);
        }
    }
}