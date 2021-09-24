using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityKeyTests
{
    [TestClass]
    public class EntityKey_PrimaryKey_GetEntity : BasePrePostTest, IGetEntityTest
    {
        [TestMethod]
        public void HasEntity()
        {
            var component = new TestPrimaryKeyComponent1 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, component);

            var entityKey = _context.WithKey(component);

            // Correct entity
            Assert.IsTrue(entityKey.HasEntity(entity));
            // Removed from withKey
            _context.RemoveComponent<TestPrimaryKeyComponent1>(entity);
            Assert.IsFalse(entityKey.HasEntity(entity));
            // Replaced from withKey
            var component2 = new TestPrimaryKeyComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity, component2);
            Assert.IsFalse(entityKey.HasEntity(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityKey.HasEntity(entity));
        }

        [TestMethod]
        public void GetEntities()
        {
            var component = new TestPrimaryKeyComponent1 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, component);

            var entityKey = _context.WithKey(component);

            // Correct entity
            Assert.IsTrue(entityKey.GetEntities().Length == 1);
            Assert.IsTrue(entityKey.GetEntities()[0] == entity);
            // Removed from withKey
            _context.RemoveComponent<TestPrimaryKeyComponent1>(entity);
            Assert.IsTrue(entityKey.GetEntities().Length == 0);
            // Replaced from withKey
            var component2 = new TestSharedKeyComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity, component2);
            Assert.IsTrue(entityKey.GetEntities().Length == 0);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityKey.GetEntities());
        }
    }
}