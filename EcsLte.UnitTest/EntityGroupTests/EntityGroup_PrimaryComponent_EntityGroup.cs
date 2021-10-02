using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityGroupTests
{
    [TestClass]
    public class EntityGroup_PrimaryComponent_EntityGroup : BasePrePostTest
    {
        [TestMethod]
        public void GetFirstOrDefault()
        {
            var component1 = new TestSharedKeyComponent1 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, component1);
            var entityGroup = _context.GroupWith(component1);

            // Correct Entity
            Assert.IsTrue(entityGroup.GetFirstOrDefault() == entity);
            // Removed from withKey
            _context.RemoveComponent<TestSharedKeyComponent1>(entity);
            Assert.IsTrue(entityGroup.GetFirstOrDefault() == Entity.Null);
            // Replaced from withKey
            var component2 = new TestSharedKeyComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity, component2);
            Assert.IsTrue(entityGroup.GetFirstOrDefault() == Entity.Null);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityGroup.GetFirstOrDefault());
        }
    }
}