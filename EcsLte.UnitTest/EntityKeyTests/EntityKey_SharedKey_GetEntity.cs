using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityKeyTests
{
    [TestClass]
    public class EntityKey_SharedKey_GetEntity : BasePrePostTest, IGetEntityTest
    {
        [TestMethod]
        public void HasEntity()
        {
            var component1 = new TestSharedKeyComponent1 { Prop = 1 };
            var entity1 = _context.CreateEntity();
            var entity2 = _context.CreateEntity();
            _context.AddComponent(entity1, component1);
            _context.AddComponent(entity2, component1);

            var entityKey = _context.WithKey(component1);

            // Correct entity
            Assert.IsTrue(entityKey.HasEntity(entity1));
            Assert.IsTrue(entityKey.HasEntity(entity2));
            // Removed from withKey
            _context.RemoveComponent<TestSharedKeyComponent1>(entity1);
            _context.RemoveComponent<TestSharedKeyComponent1>(entity2);
            Assert.IsFalse(entityKey.HasEntity(entity1));
            Assert.IsFalse(entityKey.HasEntity(entity2));
            // Replaced from withKey
            var component2 = new TestSharedKeyComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity1, component2);
            _context.ReplaceComponent(entity2, component2);
            Assert.IsFalse(entityKey.HasEntity(entity1));
            Assert.IsFalse(entityKey.HasEntity(entity2));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityKey.HasEntity(entity1));
        }

        [TestMethod]
        public void GetEntities()
        {
            var component = new TestSharedKeyComponent1 { Prop = 1 };
            var entity1 = _context.CreateEntity();
            var entity2 = _context.CreateEntity();
            _context.AddComponent(entity1, component);
            _context.AddComponent(entity2, component);

            var entityKey = _context.WithKey(component);

            // Correct entity
            Assert.IsTrue(entityKey.GetEntities().Length == 2);
            Assert.IsTrue(entityKey.GetEntities()[0] == entity1);
            Assert.IsTrue(entityKey.GetEntities()[1] == entity2);
            // Removed from withKey
            _context.RemoveComponent<TestSharedKeyComponent1>(entity1);
            _context.RemoveComponent<TestSharedKeyComponent1>(entity2);
            Assert.IsTrue(entityKey.GetEntities().Length == 0);
            // Replaced from withKey
            var component2 = new TestSharedKeyComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity1, component2);
            _context.ReplaceComponent(entity2, component2);
            Assert.IsTrue(entityKey.GetEntities().Length == 0);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityKey.GetEntities());
        }
    }
}