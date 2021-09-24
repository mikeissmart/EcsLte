using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_WithKey : BasePrePostTest, IWithKeyTest
    {
        [TestMethod]
        public void WithKey_PrimaryKey()
        {
            var component = new TestPrimaryKeyComponent1 { Prop = 1 };
            var entityKey = _context.WithKey(component);

            // Correct key
            Assert.IsTrue(entityKey != null);
            Assert.IsTrue(_context.WithKey(component) == entityKey);
            // Different component gets different entity
            Assert.IsTrue(_context.WithKey(new TestPrimaryKeyComponent1 { Prop = 2 }) != entityKey);
            // Null component
            IComponentPrimaryKey nullKey = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _context.WithKey(nullKey));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.WithKey(component));
        }

        [TestMethod]
        public void WithKey_SharedKey()
        {
            var component = new TestSharedKeyComponent1 { Prop = 1 };
            var entityKey = _context.WithKey(new TestSharedKeyComponent1 { Prop = 1 });

            // Correct key
            Assert.IsTrue(entityKey != null);
            Assert.IsTrue(_context.WithKey(component) == entityKey);
            // Different component gets different entity
            Assert.IsTrue(_context.WithKey(new TestSharedKeyComponent1 { Prop = 2 }) != entityKey);
            // Null component
            IComponentSharedKey nullKey = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _context.WithKey(nullKey));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.WithKey(component));
        }

        [TestMethod]
        public void WithKey_SharedKeyes()
        {
            var component1 = new TestSharedKeyComponent1 { Prop = 1 };
            var component2 = new TestSharedKeyComponent2 { Prop = 1 };
            var entityKey = _context.WithKey(component1, component2);

            // Correct key
            Assert.IsTrue(entityKey != null);
            Assert.IsTrue(_context.WithKey(component1, component2) == entityKey);
            // Null component
            IComponentSharedKey nullKey = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _context.WithKey(component1, component2, nullKey));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.WithKey(component1, component2));
        }
    }
}