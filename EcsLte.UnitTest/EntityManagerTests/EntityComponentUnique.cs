using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityComponentUnique : BasePrePostTest
    {
        [TestMethod]
        public void HasUniqueComponent()
        {
            var entity = _world.EntityManager.AddUniqueComponent(new TestComponentUnique1());

            // Has component
            Assert.IsTrue(_world.EntityManager.HasUniqueComponent<TestComponentUnique1>());
            // Also creates an entity
            Assert.IsTrue(_world.EntityManager.HasEntity(entity));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.HasUniqueComponent<TestComponentUnique1>());
        }

        [TestMethod]
        public void GetUniqueComponent()
        {
            var component = new TestComponentUnique1 {Prop = 1};
            _world.EntityManager.AddUniqueComponent(component);

            // Has component
            Assert.IsTrue(
                _world.EntityManager.GetUniqueComponent<TestComponentUnique1>().Prop == component.Prop);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.GetUniqueComponent<TestComponentUnique1>());
        }

        [TestMethod]
        public void GetUniqueEntity()
        {
            var entity = _world.EntityManager.AddUniqueComponent(new TestComponentUnique1());

            // Has component
            Assert.IsTrue(
                _world.EntityManager.GetUniqueEntity<TestComponentUnique1>() == entity);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.GetUniqueEntity<TestComponentUnique1>());
        }

        [TestMethod]
        public void AddUniqueComponent()
        {
            var component = new TestComponentUnique1 {Prop = 1};

            var entity = _world.EntityManager.AddUniqueComponent(component);

            // Correct component
            Assert.IsTrue(
                _world.EntityManager.GetUniqueComponent<TestComponentUnique1>().Prop == component.Prop);
            // Already has component
            Assert.ThrowsException<EntityAlreadyHasComponentUniqueException>(() =>
                _world.EntityManager.AddUniqueComponent(component));
            // Entity has component
            Assert.IsTrue(_world.EntityManager.HasComponent<TestComponentUnique1>(entity));
            Assert.IsTrue(
                _world.EntityManager.GetComponent<TestComponentUnique1>(entity).Prop == component.Prop);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.AddUniqueComponent(component));
        }

        [TestMethod]
        public void ReplaceUniqueComponent()
        {
            var component1 = new TestComponentUnique1 {Prop = 1};
            var component2 = new TestComponentUnique2 {Prop = 2};
            var entity1 = _world.EntityManager.AddUniqueComponent(new TestComponentUnique1());

            var entity2 = _world.EntityManager.ReplaceUniqueComponent(component1);

            // Correct component
            Assert.IsTrue(
                _world.EntityManager.GetUniqueComponent<TestComponentUnique1>().Prop == component1.Prop);
            // Also adds component
            _world.EntityManager.ReplaceUniqueComponent(component2);
            Assert.IsTrue(
                _world.EntityManager.GetUniqueComponent<TestComponentUnique2>().Prop == component2.Prop);
            // Add and replace entities are the same
            Assert.IsTrue(entity1 == entity2);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.ReplaceUniqueComponent(component1));
        }

        [TestMethod]
        public void RemoveUniqueComponent()
        {
            var entity = _world.EntityManager.AddUniqueComponent(new TestComponentUnique1());

            _world.EntityManager.RemoveUniqueComponent<TestComponentUnique1>();

            // Correctly removes component
            Assert.IsFalse(_world.EntityManager.HasUniqueComponent<TestComponentUnique1>());
            // Cannot remove component entiy doesnt have
            Assert.ThrowsException<EntityNotHaveComponentUniqueException>(() =>
                _world.EntityManager.RemoveUniqueComponent<TestComponentUnique1>());
            // Entity destroyed if doesnt have other components
            Assert.IsFalse(_world.EntityManager.HasEntity(entity));
            // Entity not destroyed if it has other components
            var keepEntity = _world.EntityManager.AddUniqueComponent(new TestComponentUnique2());
            _world.EntityManager.AddComponent(keepEntity, new TestComponent1());
            _world.EntityManager.RemoveUniqueComponent<TestComponentUnique2>();
            Assert.IsTrue(_world.EntityManager.HasEntity(keepEntity));
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.EntityManager.RemoveUniqueComponent<TestComponentUnique1>());
        }
        //

        [TestMethod]
        public void UniqueComponent_HasComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponentUnique1());

            // Has component
            Assert.IsTrue(_world.EntityManager.HasComponent<TestComponentUnique1>(entity));
        }

        [TestMethod]
        public void UniqueComponent_GetComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            var component = new TestComponentUnique1 {Prop = 1};
            _world.EntityManager.AddComponent(entity, component);

            // Correct component
            Assert.IsTrue(
                _world.EntityManager.GetComponent<TestComponentUnique1>(entity).Prop == component.Prop);
        }

        [TestMethod]
        public void UniqueComponent_GetAllComponents()
        {
            var entity = _world.EntityManager.CreateEntity();
            var component = new TestComponentUnique1 {Prop = 1};
            _world.EntityManager.AddComponent(entity, component);

            // Correct component count
            Assert.IsTrue(_world.EntityManager.GetAllComponents(entity).Length == 1);
        }

        [TestMethod]
        public void UniqueComponent_AddComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            var component = new TestComponentUnique1 {Prop = 1};

            _world.EntityManager.AddComponent(entity, component);

            // Correct component
            Assert.IsTrue(
                _world.EntityManager.GetUniqueComponent<TestComponentUnique1>().Prop == component.Prop);
        }

        [TestMethod]
        public void UniqueComponent_ReplaceComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            var component1 = new TestComponentUnique1 {Prop = 1};
            _world.EntityManager.AddComponent(entity, new TestComponentUnique1());

            _world.EntityManager.ReplaceComponent(entity, component1);

            // Correct component
            Assert.IsTrue(
                _world.EntityManager.GetUniqueComponent<TestComponentUnique1>().Prop == component1.Prop);
        }

        [TestMethod]
        public void UniqueComponent_RemoveComponent()
        {
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponentUnique1());

            _world.EntityManager.RemoveComponent<TestComponentUnique1>(entity);

            // Correctly removes component
            Assert.IsFalse(_world.EntityManager.HasUniqueComponent<TestComponentUnique1>());
        }

        [TestMethod]
        public void UniqueComponent_RemoveAllComponents()
        {
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestComponentUnique1());

            _world.EntityManager.RemoveAllComponents(entity);

            // Correctly removes components
            Assert.IsFalse(_world.EntityManager.HasUniqueComponent<TestComponentUnique1>());
        }
    }
}