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
        public void CreateEntityBlueprint()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var component2 = new TestComponent2 { Prop = 2 };
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2);
            var entity = _context.DefaultCommand.CreateEntity(blueprint);

            // Didnt create entity yet
            Assert.IsFalse(_context.HasEntity(entity));

            _context.DefaultCommand.RunCommands();

            // Has entity
            Assert.IsTrue(_context.HasEntity(entity));
            Assert.IsTrue(_context.GetComponent<TestComponent1>(entity).Prop == component1.Prop);
            Assert.IsTrue(_context.GetComponent<TestComponent2>(entity).Prop == component2.Prop);
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
        public void CreateEntitiesBlueprint()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var component2 = new TestComponent2 { Prop = 2 };
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2);
            var entities = _context.DefaultCommand.CreateEntities(2, blueprint);

            // Didnt create entities yet
            Assert.IsFalse(_context.HasEntity(entities[0]));
            Assert.IsFalse(_context.HasEntity(entities[1]));

            _context.DefaultCommand.RunCommands();

            // Has entity
            Assert.IsTrue(_context.HasEntity(entities[0]));
            Assert.IsTrue(_context.HasEntity(entities[1]));
            Assert.IsTrue(_context.GetComponent<TestComponent1>(entities[0]).Prop == component1.Prop);
            Assert.IsTrue(_context.GetComponent<TestComponent2>(entities[0]).Prop == component2.Prop);
            Assert.IsTrue(_context.GetComponent<TestComponent1>(entities[1]).Prop == component1.Prop);
            Assert.IsTrue(_context.GetComponent<TestComponent2>(entities[1]).Prop == component2.Prop);
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