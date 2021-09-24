using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityKeyTests
{
    [TestClass]
    public class EntityKey_PrimaryKey_EntityKey : BasePrePostTest
    {
        [TestMethod]
        public void GetFirstOrDefault()
        {
            var component1 = new TestPrimaryKeyComponent1 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, component1);
            var entityKey = _context.WithKey(component1);

            // Correct Entity
            Assert.IsTrue(entityKey.GetFirstOrDefault() == entity);
            // Removed from withKey
            _context.RemoveComponent<TestPrimaryKeyComponent1>(entity);
            Assert.IsTrue(entityKey.GetFirstOrDefault() == Entity.Null);
            // Replaced from withKey
            var component2 = new TestPrimaryKeyComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity, component2);
            Assert.IsTrue(entityKey.GetFirstOrDefault() == Entity.Null);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityKey.GetFirstOrDefault());
        }
    }
}