using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityLife : BasePrePostTest
    {
        [TestMethod]
        public void HasEntity()
        {
            var entity = _world.EntityManager.CreateEntity();

            // Has entity
            Assert.IsTrue(_world.EntityManager.HasEntity(entity));
            // Cannot have Entity.Null
            Assert.IsFalse(_world.EntityManager.HasEntity(Entity.Null));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.CreateEntity());
        }

        [TestMethod]
        public void GetEntities()
        {
            _world.EntityManager.CreateEntity();

            // Correct count
            Assert.IsTrue(_world.EntityManager.GetEntities().Length == 1);
            // Correct update count
            _world.EntityManager.CreateEntity();
            Assert.IsTrue(_world.EntityManager.GetEntities().Length == 2);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.GetEntities());
        }

        [TestMethod]
        public void CreateEntity()
        {
            var entity = _world.EntityManager.CreateEntity();

            // Has entity
            Assert.IsTrue(_world.EntityManager.HasEntity(entity));
            // Correct entity
            Assert.IsTrue(_world.EntityManager.GetEntities()[0] == entity);
            // Correct id and version
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
            // Reuse destroyed entity
            _world.EntityManager.DestroyEntity(entity);
            entity = _world.EntityManager.CreateEntity();
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 2);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.CreateEntity());
        }

        /*[TestMethod]
        public void CreateEntity_Parallel()
        {
            var entities = new Entity[TestConsts.EntityLoopCount];

            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => entities[index] = _world.EntityManager.CreateEntity());

            entities = entities.OrderBy(x => x.Id).ToArray();
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                // Has entity
                Assert.IsTrue(_world.EntityManager.HasEntity(entities[i]), $"Doesnt have entity {entities[i]}");
                // Correct entity
                Assert.IsTrue(entities[i].Id == i + 1);
                // Correct version
                Assert.IsTrue(entities[i].Version == 1);
            }
            // Correct count
            Assert.IsTrue(_world.EntityManager.GetEntities().Length == TestConsts.EntityLoopCount);
        }*/

        /*[TestMethod]
        public void CreateEntity_Reuse_Parallel()
        {
            _world.EntityManager.DestroyEntities(_world.EntityManager.CreateEntities(TestConsts.EntityLoopCount));
            var entities = new Entity[TestConsts.EntityLoopCount];

            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => entities[index] = _world.EntityManager.CreateEntity());

            entities = entities.OrderBy(x => x.Id).ToArray();
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                var entity = entities[i];
                // Has entity
                Assert.IsTrue(_world.EntityManager.HasEntity(entity));
                // Correct entity
                Assert.IsTrue(entity.Id == i + 1);
                // Correct version
                Assert.IsTrue(entities[i].Version == 2);
            }
            Assert.IsTrue(_world.EntityManager.GetEntities().Length == TestConsts.EntityLoopCount);
        }*/

        [TestMethod]
        public void CreateEntities()
        {
            var entities = _world.EntityManager.CreateEntities(2);

            // Has entity
            Assert.IsTrue(_world.EntityManager.HasEntity(entities[0]));
            Assert.IsTrue(_world.EntityManager.HasEntity(entities[1]));
            // Correct count
            Assert.IsTrue(_world.EntityManager.GetEntities().Length == 2);
            // Correct entity
            Assert.IsTrue(_world.EntityManager.GetEntities()[0] == entities[0]);
            Assert.IsTrue(_world.EntityManager.GetEntities()[1] == entities[1]);
            // Correct id and version
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[0].Version == 1);
            Assert.IsTrue(entities[1].Version == 1);
            // Different entities
            Assert.IsTrue(entities[0] != entities[1]);
            // Reuse destroyed entities
            _world.EntityManager.DestroyEntities(entities);
            entities = _world.EntityManager.CreateEntities(2);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[0].Version == 2);
            Assert.IsTrue(entities[1].Version == 2);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.CreateEntities(2));
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = _world.EntityManager.CreateEntity();

            _world.EntityManager.DestroyEntity(entity);

            // Has entity
            Assert.IsFalse(_world.EntityManager.HasEntity(entity));
            // Correct count
            Assert.IsTrue(_world.EntityManager.GetEntities().Length == 0);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.DestroyEntity(Entity.Null));
        }

        /*[TestMethod]
        public void DestroyEntity_Parallel()
        {
            var entites = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);

            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => _world.EntityManager.DestroyEntity(entites[index]));

            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                Assert.IsFalse(_world.EntityManager.HasEntity(entites[i]));
            Assert.IsTrue(_world.EntityManager.GetEntities().Length == 0);
        }*/

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = _world.EntityManager.CreateEntities(2);

            _world.EntityManager.DestroyEntities(entities);

            // Has entity
            Assert.IsFalse(_world.EntityManager.HasEntity(entities[0]));
            Assert.IsFalse(_world.EntityManager.HasEntity(entities[1]));
            // Correct count
            Assert.IsTrue(_world.EntityManager.GetEntities().Length == 0);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.DestroyEntities(entities));
        }
    }
}