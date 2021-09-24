using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_EntityLife : BasePrePostTest, IEntityLifeTest
    {
        [TestMethod]
        public void CreateEntity()
        {
            var entity = _context.CreateEntity();

            // Has entity
            Assert.IsTrue(_context.HasEntity(entity));
            // Correct entity
            Assert.IsTrue(_context.GetEntities()[0] == entity);
            // Correct id and version
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
            // Reuse destroyed entity
            _context.DestroyEntity(entity);
            entity = _context.CreateEntity();
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.CreateEntity());
        }

        [TestMethod]
        public void CreateEntities()
        {
            var entities = _context.CreateEntities(2);

            // Has entity
            Assert.IsTrue(_context.HasEntity(entities[0]));
            Assert.IsTrue(_context.HasEntity(entities[1]));
            // Correct count
            Assert.IsTrue(_context.GetEntities().Length == 2);
            // Correct entity
            Assert.IsTrue(_context.GetEntities()[0] == entities[0]);
            Assert.IsTrue(_context.GetEntities()[1] == entities[1]);
            // Correct id and version
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[0].Version == 1);
            Assert.IsTrue(entities[1].Version == 1);
            // Different entities
            Assert.IsTrue(entities[0] != entities[1]);
            // Reuse destroyed entities
            _context.DestroyEntities(entities);
            entities = _context.CreateEntities(2);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[1].Id == 2);
            Assert.IsTrue(entities[0].Version == 2);
            Assert.IsTrue(entities[1].Version == 2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.CreateEntities(2));
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = _context.CreateEntity();

            _context.DestroyEntity(entity);

            // Has entity
            Assert.IsFalse(_context.HasEntity(entity));
            // Correct count
            Assert.IsTrue(_context.GetEntities().Length == 0);
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _context.DestroyEntity(Entity.Null));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DestroyEntity(entity));
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = _context.CreateEntities(2);

            _context.DestroyEntities(entities);

            // Has entity
            Assert.IsFalse(_context.HasEntity(entities[0]));
            Assert.IsFalse(_context.HasEntity(entities[1]));
            // Correct count
            Assert.IsTrue(_context.GetEntities().Length == 0);
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _context.DestroyEntities(new[] { Entity.Null }));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DestroyEntities(entities));
        }
    }
}