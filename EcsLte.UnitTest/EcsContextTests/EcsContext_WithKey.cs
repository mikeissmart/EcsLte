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
        public void GroupWith_SharedKey()
        {
            var component = new TestSharedKeyComponent1 {Prop = 1};
            var entityKey = _context.GroupWith(new TestSharedKeyComponent1 {Prop = 1});

            // Correct key
            Assert.IsTrue(entityKey != null);
            Assert.IsTrue(_context.GroupWith(component) == entityKey);
            // Different component gets different entity
            Assert.IsTrue(_context.GroupWith(new TestSharedKeyComponent1 {Prop = 2}) != entityKey);
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
        public void GroupWith_SharedKeyes()
        {
            var component1 = new TestSharedKeyComponent1 {Prop = 1};
            var component2 = new TestSharedKeyComponent2 {Prop = 1};
            var entityKey = _context.GroupWith(component1, component2);

            // Correct key
            Assert.IsTrue(entityKey != null);
            Assert.IsTrue(_context.GroupWith(component1, component2) == entityKey);
            // Null component
            ISharedComponent nullKey = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _context.GroupWith(component1, component2, nullKey));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.GroupWith(component1, component2));
        }
    }
}