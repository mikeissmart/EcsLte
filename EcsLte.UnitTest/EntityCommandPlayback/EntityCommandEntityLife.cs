using System;
using System.Threading;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandPlayback
{
    [TestClass]
    public class EntityCommandEntityLife
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
            var entity = world.EntityManager.DefaultEntityCommandPlayback.CreateEntity();
            Assert.IsFalse(world.EntityManager.HasEntity(entity));

            world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsTrue(world.EntityManager.HasEntity(entity));
            Assert.IsTrue(entity.Id == 1, $"Entity.id = {entity.Id}, Entities:  " +
                String.Join(", ", world.EntityManager.GetEntities()));
        }

        [TestMethod]
        public void CreateEntities()
        {
            var world = World.DefaultWorld;
            var entities = world.EntityManager.DefaultEntityCommandPlayback
                .CreateEntities(TestConsts.EntityLoopCount);

            entities = entities.OrderBy(x => x.Id).ToArray();
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                Assert.IsFalse(world.EntityManager.HasEntity(entities[i]));

            world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                Assert.IsTrue(world.EntityManager.HasEntity(entities[i]));
                Assert.IsTrue(entities[i].Id == i + 1);
            }
        }

        [TestMethod]
        public void Destroy()
        {
            var world = World.DefaultWorld;
            var entity = world.EntityManager.CreateEntity();

            world.EntityManager.DefaultEntityCommandPlayback.DestroyEntity(entity);
            Assert.IsTrue(world.EntityManager.HasEntity(entity));

            world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsFalse(world.EntityManager.HasEntity(entity));
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var world = World.DefaultWorld;
            var entities = world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);

            entities = entities.OrderBy(x => x.Id).ToArray();
            world.EntityManager.DefaultEntityCommandPlayback.DestroyEntities(entities);
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                Assert.IsTrue(world.EntityManager.HasEntity(entities[i]));

            world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                Assert.IsFalse(world.EntityManager.HasEntity(entities[i]));
                Assert.IsTrue(entities[i].Id == i + 1);
            }
        }
    }
}