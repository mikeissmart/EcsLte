using System;
using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.KeyTests
{
    [TestClass]
    public class KeyEntityTest : BasePrePostTest
    {
        [TestMethod]
        public void PrimaryKeyGetEntity()
        {
            var entity1 = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity1, new TestPrimaryKeyComponent1());
            var key = _world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>();

            _world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>()
                .GetEntity(new TestPrimaryKeyComponent1(), out var entity2);

            // Correct entity
            Assert.IsTrue(entity1 == entity2);
            // Wrong type
            Assert.ThrowsException<PrimaryKeyWrongTypeException>(() =>
                ((IPrimaryKey)key).GetEntity(new TestComponent1(), out var invalidEntity));
            // Still has same count
            var entity3 = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity3, new TestSharedKeyComponent1 { Prop = 1 });
            _world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>()
                .GetEntity(new TestPrimaryKeyComponent1(), out var entity4);
            Assert.IsTrue(entity1 == entity4);
        }

        [TestMethod]
        public void SharedKeyGetEntities()
        {
            var entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity, new TestSharedKeyComponent1());
            var key = _world.KeyManager.GetSharedKey<TestSharedKeyComponent1>();

            var entities = _world.KeyManager.GetSharedKey<TestSharedKeyComponent1>()
                .GetEntities(new TestSharedKeyComponent1());

            // Correct count
            Assert.IsTrue(entities.Length == 1);
            // Correct entity
            Assert.IsTrue(entities[0] == entity);
            // Wrong type
            Assert.ThrowsException<SharedKeyWrongTypeException>(() =>
                ((ISharedKey)key).GetEntities(new TestComponent1()));
            // Still has same count
            var entity2 = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponent(entity2, new TestSharedKeyComponent1 { Prop = 1 });
            Assert.IsTrue(_world.KeyManager.GetSharedKey<TestSharedKeyComponent1>()
                .GetEntities(new TestSharedKeyComponent1()).Length == 1);
        }
    }
}