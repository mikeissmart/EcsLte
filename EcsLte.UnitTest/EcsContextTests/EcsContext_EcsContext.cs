using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_EcsContext : BasePrePostTest
    {
        [TestMethod]
        public void DefaultCommand()
        {
            // Has default entity command queue
            Assert.IsTrue(_context.DefaultCommand != null);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.IsTrue(_context.DefaultCommand != null);
        }

        [TestMethod]
        public void AddUniqueComponent()
        {
            var component = new TestComponentUnique1 { Prop = 1 };

            _context.AddUniqueComponent(component);

            // Correct component
            Assert.IsTrue(
                _context.GetUniqueComponent<TestComponentUnique1>().Prop == component.Prop);
            // Already has component
            Assert.ThrowsException<EntityAlreadyHasComponentUniqueException>(() =>
                _context.AddUniqueComponent(component));
            // Unable to add same unique component to another entity
            Assert.ThrowsException<EntityAlreadyHasComponentUniqueException>(() =>
                _context.AddUniqueComponent(component));
            // Can add to another entity once exisitng unique component is removed
            _context.RemoveUniqueComponent<TestComponentUnique1>();
            _context.AddUniqueComponent(component);
            Assert.IsTrue(_context.HasUniqueComponent<TestComponentUnique1>());
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.AddUniqueComponent(new TestComponentUnique1()));
        }

        [TestMethod]
        public void ReplaceUniqueComponent()
        {
            var component1 = new TestComponentUnique1 { Prop = 1 };
            _context.AddUniqueComponent(new TestComponentUnique1());

            _context.ReplaceUniqueComponent(component1);

            // Correct component
            Assert.IsTrue(
                _context.GetUniqueComponent<TestComponentUnique1>().Prop == component1.Prop);
            // Also adds component
            var component2 = new TestComponentUnique2 { Prop = 2 };
            _context.ReplaceUniqueComponent(component2);
            Assert.IsTrue(
                _context.GetUniqueComponent<TestComponentUnique2>().Prop == component2.Prop);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.ReplaceUniqueComponent(new TestComponentUnique1()));
        }

        [TestMethod]
        public void RemoveUniqueComponent()
        {
            _context.AddUniqueComponent(new TestComponentUnique1());

            _context.RemoveUniqueComponent<TestComponentUnique1>();

            // Correctly removes component
            Assert.IsFalse(_context.HasUniqueComponent<TestComponentUnique1>());
            // Cannot remove component entiy doesnt have
            Assert.ThrowsException<EntityNotHaveComponentUniqueException>(() =>
                _context.RemoveUniqueComponent<TestComponentUnique1>());
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.RemoveUniqueComponent<TestComponentUnique1>());
        }

        [TestMethod]
        public void CommandQueue()
        {
            var command1 = _context.CommandQueue("Test1");

            // Correctly command queue
            Assert.IsTrue(command1 != null);
            // Get same command queue
            var command2 = _context.CommandQueue("Test1");
            Assert.IsTrue(command1 == command2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.CommandQueue("Test1"));
        }
    }
}