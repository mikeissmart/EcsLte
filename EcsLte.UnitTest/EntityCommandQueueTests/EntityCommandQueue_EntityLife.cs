using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandQueueTests
{
    [TestClass]
    public class EntityCommandQueue_EntityLife : BasePrePostTest, IEntityLifeTest
    {
        [TestMethod]
        public void CreateEntity()
        {
            var entity = _context.DefaultCommand.CreateEntity();

            // Didnt create entity yet
            Assert.IsFalse(_context.HasEntity(entity));

            _context.DefaultCommand.RunCommands();

            // Has entity
            Assert.IsTrue(_context.HasEntity(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.CreateEntity());
        }

        [TestMethod]
        public void CreateEntities()
        {
            var entities = _context.DefaultCommand.CreateEntities(2);

            // Didnt create entities yet
            Assert.IsFalse(_context.HasEntity(entities[0]));
            Assert.IsFalse(_context.HasEntity(entities[1]));

            _context.DefaultCommand.RunCommands();

            // Has entity
            Assert.IsTrue(_context.HasEntity(entities[0]));
            Assert.IsTrue(_context.HasEntity(entities[1]));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.CreateEntities(2));
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = _context.CreateEntity();

            _context.DefaultCommand.DestroyEntity(entity);

            // Didnt destroy entity yet
            Assert.IsTrue(_context.HasEntity(entity));

            _context.DefaultCommand.RunCommands();

            // Has entity
            Assert.IsFalse(_context.HasEntity(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.DestroyEntity(entity));
        }

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = _context.CreateEntities(2);

            _context.DefaultCommand.DestroyEntities(entities);

            // Didnt destroy entities yet
            Assert.IsTrue(_context.HasEntity(entities[0]));
            Assert.IsTrue(_context.HasEntity(entities[1]));

            _context.DefaultCommand.RunCommands();

            // Has entity
            Assert.IsFalse(_context.HasEntity(entities[0]));
            Assert.IsFalse(_context.HasEntity(entities[1]));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.DestroyEntities(entities));
        }
    }
}