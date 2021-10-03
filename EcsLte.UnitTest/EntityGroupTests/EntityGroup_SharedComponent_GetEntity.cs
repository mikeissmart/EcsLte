using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityGroupTests
{
    [TestClass]
    public class EntityGroup_SharedComponent_GetEntity : BasePrePostTest, IGetEntityTest
    {
        [TestMethod]
        public void HasEntity()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var entity1 = _context.CreateEntity();
            var entity2 = _context.CreateEntity();
            _context.AddComponent(entity1, component1);
            _context.AddComponent(entity2, component1);

            var entityGroup = _context.GroupWith(component1);

            // Correct entity
            Assert.IsTrue(entityGroup.HasEntity(entity1));
            Assert.IsTrue(entityGroup.HasEntity(entity2));
            // Removed from withKey
            _context.RemoveComponent<TestSharedComponent1>(entity1);
            _context.RemoveComponent<TestSharedComponent1>(entity2);
            Assert.IsFalse(entityGroup.HasEntity(entity1));
            Assert.IsFalse(entityGroup.HasEntity(entity2));
            // Replaced from withKey
            var component2 = new TestSharedComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity1, component2);
            _context.ReplaceComponent(entity2, component2);
            Assert.IsFalse(entityGroup.HasEntity(entity1));
            Assert.IsFalse(entityGroup.HasEntity(entity2));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityGroup.HasEntity(entity1));
        }

        [TestMethod]
        public void GetEntities()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            var entity1 = _context.CreateEntity();
            var entity2 = _context.CreateEntity();
            _context.AddComponent(entity1, component);
            _context.AddComponent(entity2, component);

            var entityGroup = _context.GroupWith(component);

            // Correct entity
            Assert.IsTrue(entityGroup.GetEntities().Length == 2);
            Assert.IsTrue(entityGroup.GetEntities()[0] == entity1);
            Assert.IsTrue(entityGroup.GetEntities()[1] == entity2);
            // Removed from withKey
            _context.RemoveComponent<TestSharedComponent1>(entity1);
            _context.RemoveComponent<TestSharedComponent1>(entity2);
            Assert.IsTrue(entityGroup.GetEntities().Length == 0);
            // Replaced from withKey
            var component2 = new TestSharedComponent1 { Prop = 2 };
            _context.ReplaceComponent(entity1, component2);
            _context.ReplaceComponent(entity2, component2);
            Assert.IsTrue(entityGroup.GetEntities().Length == 0);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                entityGroup.GetEntities());
        }
    }
}