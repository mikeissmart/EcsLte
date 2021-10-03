using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandQueueTests
{
    [TestClass]
    public class EntityCommandQueue_ComponentLife : BasePrePostTest, IComponentLifeTest
    {
        [TestMethod]
        public void AddUniqueComponent()
        {
            var entity = _context.CreateEntity();
            var component = new TestUniqueComponent1 { Prop = 1 };

            _context.DefaultCommand.AddUniqueComponent(entity, component);

            // Doesnt have component yet
            Assert.IsFalse(_context.HasUniqueComponent<TestUniqueComponent1>());

            _context.DefaultCommand.RunCommands();

            // Correct component
            Assert.IsTrue(_context.HasUniqueComponent<TestUniqueComponent1>());
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.AddUniqueComponent(entity, new TestUniqueComponent1()));
        }

        [TestMethod]
        public void ReplaceUniqueComponent()
        {
            var component1 = new TestUniqueComponent1 { Prop = 1 };
            var entity = _context.AddUniqueComponent(new TestUniqueComponent1());

            _context.DefaultCommand.ReplaceUniqueComponent(entity, component1);

            // Didnt update component yet
            Assert.IsFalse(_context.GetUniqueComponent<TestUniqueComponent1>().Prop == component1.Prop);

            _context.DefaultCommand.RunCommands();

            // Correct component
            Assert.IsTrue(_context.GetUniqueComponent<TestUniqueComponent1>().Prop == component1.Prop);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.ReplaceUniqueComponent(entity, new TestUniqueComponent1()));
        }

        [TestMethod]
        public void RemoveUniqueComponent()
        {
            var entity = _context.CreateEntity();
            var component1 = new TestUniqueComponent1 { Prop = 1 };
            _context.AddUniqueComponent(entity, new TestUniqueComponent1());

            _context.DefaultCommand.RemoveUniqueComponent<TestUniqueComponent1>(entity);

            // Didnt remove component yet
            Assert.IsTrue(_context.HasUniqueComponent<TestUniqueComponent1>());

            _context.DefaultCommand.RunCommands();

            // Removed component
            Assert.IsFalse(_context.HasUniqueComponent<TestUniqueComponent1>());
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.RemoveUniqueComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void AddComponent()
        {
            var entity = _context.CreateEntity();

            _context.DefaultCommand.AddComponent(entity, new TestComponent1());

            // Didnt add component yet
            Assert.IsFalse(_context.HasComponent<TestComponent1>(entity));

            _context.DefaultCommand.RunCommands();

            // Correct component
            Assert.IsTrue(_context.HasComponent<TestComponent1>(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.AddComponent(entity, new TestComponent1()));
        }

        [TestMethod]
        public void ReplaceComponent()
        {
            var entity = _context.CreateEntity();
            var component = new TestComponent1 { Prop = 1 };
            _context.AddComponent(entity, new TestComponent1());

            _context.DefaultCommand.ReplaceComponent(entity, component);

            // Didnt update component yet
            Assert.IsFalse(_context.GetComponent<TestComponent1>(entity).Prop == component.Prop);

            _context.DefaultCommand.RunCommands();

            // Correct component
            Assert.IsTrue(_context.GetComponent<TestComponent1>(entity).Prop == component.Prop);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.ReplaceComponent(entity, component));
        }

        [TestMethod]
        public void RemoveComponent()
        {
            var entity = _context.CreateEntity();
            var component = new TestComponent1 { Prop = 1 };
            _context.AddComponent(entity, new TestComponent1());

            _context.DefaultCommand.RemoveComponent<TestComponent1>(entity);

            // Didnt remove component yet
            Assert.IsTrue(_context.HasComponent<TestComponent1>(entity));

            _context.DefaultCommand.RunCommands();

            // Correct component
            Assert.IsFalse(_context.HasComponent<TestComponent1>(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.RemoveComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void RemoveAllComponents()
        {
            var entity = _context.CreateEntity();
            var component = new TestComponent1 { Prop = 1 };
            _context.AddComponent(entity, new TestComponent1());
            _context.AddComponent(entity, new TestComponent2());

            _context.DefaultCommand.RemoveAllComponents(entity);

            // Didnt remove components yet
            Assert.IsTrue(_context.HasComponent<TestComponent1>(entity));
            Assert.IsTrue(_context.HasComponent<TestComponent2>(entity));

            _context.DefaultCommand.RunCommands();

            // Correct component
            Assert.IsFalse(_context.HasComponent<TestComponent1>(entity));
            Assert.IsFalse(_context.HasComponent<TestComponent2>(entity));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.DefaultCommand.RemoveAllComponents(entity));
        }
    }
}