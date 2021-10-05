using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityFilterTests
{
    [TestClass]
    public class EntityFilter_GetEntity : BasePrePostTest
    {
        [TestMethod]
        public void HasEntity()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, new TestComponent1());

            // Has entity
            Assert.IsTrue(filter.HasEntity(entity));
            // Removes entity when not filtered
            _context.RemoveComponent<TestComponent1>(entity);
            Assert.IsFalse(filter.HasEntity(entity));
            // Destroy entity removes from filter
            _context.AddComponent(entity, new TestSharedComponent1());
            _context.DestroyEntity(entity);
            Assert.IsFalse(filter.HasEntity(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.HasEntity(entity));
        }

        [TestMethod]
        public void GetEntities()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var entity1 = _context.CreateEntity();
            var entity2 = _context.CreateEntity();
            _context.AddComponent(entity1, new TestComponent1());
            _context.AddComponent(entity2, new TestComponent1());

            // Has entity
            Assert.IsTrue(filter.GetEntities().Length == 2);
            Assert.IsTrue(filter.GetEntities()[0] == entity1);
            Assert.IsTrue(filter.GetEntities()[1] == entity2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.GetEntities());
        }
    }
}