using System.Threading;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManangerTests
{
    [TestClass]
    public class EntityLife
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
            var eventCalled = false;
            var world = World.DefaultWorld;

            world.EntityManager.AnyEntityCreated.Subscribe(entity => eventCalled = true);
            var entity = world.EntityManager.CreateEntity();

            Assert.IsTrue(world.EntityManager.HasEntity(entity));
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void Create_Parallel()
        {
            var eventCalled = 0;
            var world = World.DefaultWorld;
            var entities = new Entity[TestConsts.EntityLoopCount];

            world.EntityManager.AnyEntityCreated.Subscribe(entity => Interlocked.Increment(ref eventCalled));
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => entities[index] = world.EntityManager.CreateEntity());

            entities = entities.OrderBy(x => x.Id).ToArray();
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                Assert.IsTrue(world.EntityManager.HasEntity(entities[i]));
                Assert.IsTrue(entities[i].Id == i + 1);
            }

            Assert.IsTrue(eventCalled == TestConsts.EntityLoopCount);
        }

        [TestMethod]
        public void CreateAfterWorldDestroy()
        {
            var world = World.DefaultWorld;
            World.DestroyWorld(world);

            Assert.ThrowsException<WorldIsDestroyedException>(() => world.EntityManager.CreateEntity());
        }

        [TestMethod]
        public void CreateNoWorld()
        {
            Assert.IsFalse(World.DefaultWorld.EntityManager.HasEntity(new Entity()));
        }

        [TestMethod]
        public void CreateEntities()
        {
            var eventCalled = 0;
            var world = World.DefaultWorld;

            world.EntityManager.AnyEntityCreated.Subscribe(entity => eventCalled++);
            var entities = world.EntityManager.CreateEntities(2);

            Assert.IsTrue(world.EntityManager.HasEntity(entities[0]));
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(world.EntityManager.HasEntity(entities[1]));
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[0] != entities[1]);
            Assert.IsTrue(eventCalled == 2);
        }

        [TestMethod]
        public void CreateReuse()
        {
            var world = World.DefaultWorld;
            world.EntityManager.DestroyEntity(world.EntityManager.CreateEntity());

            var entity = world.EntityManager.CreateEntity();

            Assert.IsTrue(world.EntityManager.HasEntity(entity) && entity.Version == 2);
        }

        [TestMethod]
        public void CreateReuse_Parallel()
        {
            var world = World.DefaultWorld;
            world.EntityManager.DestroyEntities(world.EntityManager.CreateEntities(TestConsts.EntityLoopCount));
            var entities = new Entity[TestConsts.EntityLoopCount];

            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => entities[index] = world.EntityManager.CreateEntity());

            entities = entities.OrderBy(x => x.Id).ToArray();
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(world.EntityManager.HasEntity(entity));
                Assert.IsTrue(entity.Id == i + 1);
                Assert.IsTrue(entity.Version == 2);
            }
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
        public void Destroy_Parallel()
        {
            var eventCalled = 0;
            var world = World.DefaultWorld;
            var entites = world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);

            world.EntityManager.AnyEntityWillBeDestroyedEvent.Subscribe(entity => Interlocked.Increment(ref eventCalled));
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => world.EntityManager.DestroyEntity(entites[index]));

            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                Assert.IsFalse(world.EntityManager.HasEntity(entites[i]));
            Assert.IsTrue(eventCalled == TestConsts.EntityLoopCount);
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var eventCalled = 0;
            var world = World.DefaultWorld;
            var entities = world.EntityManager.CreateEntities(2);

            world.EntityManager.AnyEntityWillBeDestroyedEvent.Subscribe(entity => eventCalled++);
            world.EntityManager.DestroyEntities(entities);

            Assert.IsFalse(world.EntityManager.HasEntity(entities[0]));
            Assert.IsFalse(world.EntityManager.HasEntity(entities[1]));
            Assert.IsTrue(eventCalled == 2);
        }

        [TestMethod]
        public void DestroyAfterWorldDestroy()
        {
            var world = World.DefaultWorld;
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
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            World.DestroyWorld(world);

            Assert.ThrowsException<WorldIsDestroyedException>(() => World.DestroyWorld(world));
        }
    }
}