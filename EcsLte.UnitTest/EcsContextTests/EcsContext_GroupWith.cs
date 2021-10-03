using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_GroupWith : BasePrePostTest, IGroupWithTest
    {
        [TestMethod]
        public void GroupWith_SharedComponent()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            var group = _context.GroupWith(new TestSharedComponent1 { Prop = 1 });

            // Correct group
            Assert.IsTrue(group != null);
            Assert.IsTrue(_context.GroupWith(component) == group);
            // Different component gets different entity
            Assert.IsTrue(_context.GroupWith(new TestSharedComponent1 { Prop = 2 }) != group);
            // Null component
            ISharedComponent nullKey = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _context.GroupWith(nullKey));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.GroupWith(component));
        }

        [TestMethod]
        public void GroupWith_SharedComponents()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 1 };
            var group = _context.GroupWith(component1, component2);

            // Correct group
            Assert.IsTrue(group != null);
            Assert.IsTrue(_context.GroupWith(component1, component2) == group);
            // Null component
            ISharedComponent nullComponent = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _context.GroupWith(component1, component2, nullComponent));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.GroupWith(component1, component2));
        }
    }
}