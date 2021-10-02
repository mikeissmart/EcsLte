using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityGroupTests
{
    [TestClass]
    public class EntityGroup_PrimaryComponent_GetEntity : BasePrePostTest, IGetEntityTest
    {
        [TestMethod]
        public void HasEntity()
        {
            var component = new TestSharedKeyComponent1 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, component);

            var entityGroup = _context.GroupWith(component);

            // Correct entity
            Assert.IsTrue(entityGroup.HasEntity(entity));
            // Removed from withKey
            _context.RemoveComponent<TestSharedKeyComponent1>(entity);
            Assert.IsFalse(entityGroup.HasEntity(entity));
            // Replaced from withKey
            var component2 = new TestSharedKeyComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity, component2);
            Assert.IsFalse(entityGroup.HasEntity(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityGroup.HasEntity(entity));
        }

        [TestMethod]
        public void GetEntities()
        {
            var component = new TestSharedKeyComponent1 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, component);

            var entityGroup = _context.GroupWith(component);

            // Correct entity
            Assert.IsTrue(entityGroup.GetEntities().Length == 1);
            Assert.IsTrue(entityGroup.GetEntities()[0] == entity);
            // Removed from withKey
            _context.RemoveComponent<TestSharedKeyComponent1>(entity);
            Assert.IsTrue(entityGroup.GetEntities().Length == 0);
            // Replaced from withKey
            var component2 = new TestSharedKeyComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity, component2);
            Assert.IsTrue(entityGroup.GetEntities().Length == 0);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityGroup.GetEntities());
        }
    }
}