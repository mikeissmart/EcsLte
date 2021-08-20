using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManangerTests
{
    [TestClass]
    public class EntityLife
    {
        [TestInitialize]
        public void PreTest()
        {
            World.DefaultWorld = World.DefaultWorld == null
                ? World.CreateWorld("DefaultWorld")
                : World.DefaultWorld;
        }

        [TestCleanup]
        public void PostTest()
        {
            World.DestroyWorld(World.DefaultWorld);
            World.DefaultWorld = null;
        }

        [TestMethod]
        public void Create()
        {
            var eventCalled = false;
            var world = World.DefaultWorld;

            world.EntityManager.AnyEntityCreated.Subscribe(entity => eventCalled = true);
            var entity = World.DefaultWorld.EntityManager.CreateEntity();

            Assert.IsTrue(world.EntityManager.HasEntity(entity) && entity.Id != 0);
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void CreateAfterWorldDestroy()
        {
            var world = World.CreateWorld("CreateAfterWorldDestroy");
            World.DestroyWorld(world);

            Assert.ThrowsException<WorldIsDestroyedException>(() => world.EntityManager.CreateEntity());
        }

        [TestMethod]
        public void CreateNoWorld()
        {
            Assert.IsFalse(World.DefaultWorld.EntityManager.HasEntity(new Entity()));
        }

        [TestMethod]
        public void CreateMultiple()
        {
            var eventCalled = 0;
            var world = World.DefaultWorld;

            world.EntityManager.AnyEntityCreated.Subscribe(entity => eventCalled++);
            var entities = world.EntityManager.CreateEntities(2);

            Assert.IsTrue(world.EntityManager.HasEntity(entities[0]) && entities[0].Id != 0);
            Assert.IsTrue(world.EntityManager.HasEntity(entities[1]) && entities[1].Id != 0);
            Assert.IsTrue(entities[0] != entities[1]);
            Assert.IsTrue(eventCalled == 2);
        }

        [TestMethod]
        public void CreateReuse()
        {
            var world = World.DefaultWorld;
            world.EntityManager.DestroyEntity(world.EntityManager.CreateEntity());

            var entity = world.EntityManager.CreateEntity();

            Assert.IsTrue(world.EntityManager.HasEntity(entity) && entity.Version != 1);
        }

        [TestMethod]
        public void Destroy()
        {
            var eventCalled = false;
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.AnyEntityWillBeDestroyedEvent.Subscribe(entity => eventCalled = true);
            world.EntityManager.DestroyEntity(entity);

            Assert.IsFalse(world.EntityManager.HasEntity(entity));
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void DestroyAll()
        {
            var eventCalled = 0;
            var world = World.DefaultWorld;

            world.EntityManager.CreateEntities(2);
            world.EntityManager.AnyEntityWillBeDestroyedEvent.Subscribe(entity => eventCalled++);
            world.EntityManager.DestroyAllEntities();

            Assert.IsTrue(world.EntityManager.GetEntities().Length == 0);
            Assert.IsTrue(eventCalled == 2);
        }

        [TestMethod]
        public void DestroyAfterWorldDestroy()
        {
            var world = World.CreateWorld("DestroyAfterWorldDestroy");
            var entity = world.EntityManager.CreateEntity();
            World.DestroyWorld(world);

            Assert.ThrowsException<WorldIsDestroyedException>(() => world.EntityManager.DestroyEntity(entity));
        }

        [TestMethod]
        public void DestroyMultiple()
        {
            var world = World.DefaultWorld;
            var entities = World.DefaultWorld.EntityManager.CreateEntities(2);
            world.EntityManager.DestroyEntities(entities);

            Assert.IsFalse(world.EntityManager.HasEntity(entities[0]) &&
                           world.EntityManager.HasEntity(entities[1]));
        }

        [TestMethod]
        public void DestroyWorld()
        {
            var world = World.CreateWorld("DestroyWorld");
            var entity = world.EntityManager.CreateEntity();

            World.DestroyWorld(world);

            Assert.ThrowsException<WorldIsDestroyedException>(() => world.EntityManager.HasEntity(entity));
        }
    }
}