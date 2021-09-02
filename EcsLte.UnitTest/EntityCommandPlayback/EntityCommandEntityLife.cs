using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandPlayback
{
    [TestClass]
    public class EntityCommandEntityLife : BasePrePostTest
    {
        [TestMethod]
        public void Create()
        {
            var entity = _world.EntityManager.DefaultEntityCommandPlayback.CreateEntity();
            Assert.IsFalse(_world.EntityManager.HasEntity(entity));

            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsTrue(_world.EntityManager.HasEntity(entity));
            Assert.IsTrue(entity.Id == 1, $"Entity.id = {entity.Id}, Entities:  " +
                string.Join(", ", _world.EntityManager.GetEntities()));
        }

        [TestMethod]
        public void CreateEntities()
        {
            var entities = _world.EntityManager.DefaultEntityCommandPlayback
                .CreateEntities(TestConsts.EntityLoopCount);

            entities = entities.OrderBy(x => x.Id).ToArray();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                Assert.IsFalse(_world.EntityManager.HasEntity(entities[i]));

            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                Assert.IsTrue(_world.EntityManager.HasEntity(entities[i]));
                Assert.IsTrue(entities[i].Id == i + 1);
            }
        }

        [TestMethod]
        public void Destroy()
        {
            var entity = _world.EntityManager.CreateEntity();

            _world.EntityManager.DefaultEntityCommandPlayback.DestroyEntity(entity);
            Assert.IsTrue(_world.EntityManager.HasEntity(entity));

            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsFalse(_world.EntityManager.HasEntity(entity));
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);

            entities = entities.OrderBy(x => x.Id).ToArray();
            _world.EntityManager.DefaultEntityCommandPlayback.DestroyEntities(entities);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                Assert.IsTrue(_world.EntityManager.HasEntity(entities[i]));

            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                Assert.IsFalse(_world.EntityManager.HasEntity(entities[i]));
                Assert.IsTrue(entities[i].Id == i + 1);
            }
        }
    }
}