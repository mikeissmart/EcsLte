using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_GetComponent : BasePrePostTest, IGetComponentTest
    {
        [TestMethod]
        public void HasUniqueComponent()
        {
            var entity = _context.CreateEntity();
            _context.AddUniqueComponent(entity, new TestComponentUnique1());

            // Has unique component
            Assert.IsTrue(_context.HasUniqueComponent<TestComponentUnique1>());
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.HasUniqueComponent<TestComponentUnique1>());
        }

        [TestMethod]
        public void GetUniqueComponent()
        {
            var component1 = new TestComponentUnique1 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddUniqueComponent(entity, component1);

            var component2 = _context.GetUniqueComponent<TestComponentUnique1>();

            // Correct unique component
            Assert.IsTrue(component1.Equals(component2));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.GetUniqueComponent<TestComponentUnique1>());
        }

        [TestMethod]
        public void GetUniqueEntity()
        {
            var component1 = new TestComponentUnique1 { Prop = 1 };
            var entity1 = _context.CreateEntity();
            _context.AddUniqueComponent(entity1, component1);

            var entity2 = _context.GetUniqueEntity<TestComponentUnique1>();
            var component2 = _context.GetComponent<TestComponentUnique1>(entity1);

            // Correct unique component
            Assert.IsTrue(entity1 == entity2);
            // Correct entity component
            Assert.IsTrue(component1.Equals(component2));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.GetUniqueEntity<TestComponentUnique1>());
        }

        [TestMethod]
        public void HasComponent()
        {
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, new TestComponent1());

            // Has component
            Assert.IsTrue(_context.HasComponent<TestComponent1>(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void GetComponent()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, component1);

            var component2 = _context.GetComponent<TestComponent1>(entity);

            // Correct unique component
            Assert.IsTrue(component1.Equals(component2));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.GetComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void GetAllComponents()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var component2 = new TestComponent2 { Prop = 1 };
            var entity = _context.CreateEntity();
            _context.AddComponent(entity, component1);
            _context.AddComponent(entity, component2);

            var components = _context.GetAllComponents(entity);

            // Correct component count
            Assert.IsTrue(components.Length == 2);
            // Correct components
            Assert.IsTrue(components[0] is TestComponent1);
            Assert.IsTrue(components[1] is TestComponent2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.GetAllComponents(entity));
        }
    }
}