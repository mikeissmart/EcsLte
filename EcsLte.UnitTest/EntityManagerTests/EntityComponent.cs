using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityComponent : BasePrePostTest
    {
        [TestMethod]
        public void HasComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            // Has component
            Assert.IsTrue(_world.EntityManager.HasComponent<TestComponent1>(entity));
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _world.EntityManager.HasComponent<TestComponent1>(Entity.Null));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.HasComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void GetComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            var component = new TestComponent1 {Prop = 1};
            _world.EntityManager.AddComponent(entity, component);

            // Correct component
            Assert.IsTrue(
                _world.EntityManager.GetComponent<TestComponent1>(entity).Prop == component.Prop);
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _world.EntityManager.GetComponent<TestComponent1>(Entity.Null));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.GetComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void GetAllComponents()
        {
            var entity = _world.EntityManager.CreateEntity();
            var component = new TestComponent1 {Prop = 1};
            _world.EntityManager.AddComponent(entity, component);

            // Correct component count
            Assert.IsTrue(_world.EntityManager.GetAllComponents(entity).Length == 1);
            // Correct component
            Assert.IsTrue(((TestComponent1) _world.EntityManager.GetAllComponents(entity)[0]).Prop == component.Prop);
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _world.EntityManager.GetAllComponents(Entity.Null));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.GetAllComponents(Entity.Null));
        }

        [TestMethod]
        public void AddComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            var component = new TestComponent1 {Prop = 1};

            _world.EntityManager.AddComponent(entity, component);

            // Correct component
            Assert.IsTrue(
                _world.EntityManager.GetComponent<TestComponent1>(entity).Prop == component.Prop);
            // Already has component
            Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
                _world.EntityManager.AddComponent(entity, component));
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _world.EntityManager.AddComponent(Entity.Null, component));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.AddComponent(Entity.Null, component));
        }

        [TestMethod]
        public void ReplaceComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            var component1 = new TestComponent1 {Prop = 1};
            var component2 = new TestComponent2 {Prop = 2};
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            _world.EntityManager.ReplaceComponent(entity, component1);

            // Correct component
            Assert.IsTrue(
                _world.EntityManager.GetComponent<TestComponent1>(entity).Prop == component1.Prop);
            // Also adds component
            _world.EntityManager.ReplaceComponent(entity, component2);
            Assert.IsTrue(
                _world.EntityManager.GetComponent<TestComponent2>(entity).Prop == component2.Prop);
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _world.EntityManager.ReplaceComponent(Entity.Null, component1));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.ReplaceComponent(Entity.Null, component1));
        }

        [TestMethod]
        public void RemoveComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            _world.EntityManager.RemoveComponent<TestComponent1>(entity);

            // Correctly removes component
            Assert.IsFalse(_world.EntityManager.HasComponent<TestComponent1>(entity));
            // Cannot remove component entiy doesnt have
            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                _world.EntityManager.RemoveComponent<TestComponent1>(entity));
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _world.EntityManager.RemoveComponent<TestComponent1>(Entity.Null));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.RemoveComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void RemoveAllComponents()
        {
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponent1());

            _world.EntityManager.RemoveAllComponents(entity);

            // Correctly removes components
            Assert.IsTrue(_world.EntityManager.GetAllComponents(entity).Length == 0);
            Assert.IsFalse(_world.EntityManager.HasComponent<TestComponent1>(entity));
            // Entity does not exist
            Assert.ThrowsException<EntityDoesNotExistException>(() =>
                _world.EntityManager.RemoveAllComponents(Entity.Null));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.RemoveAllComponents(Entity.Null));
        }
    }
}