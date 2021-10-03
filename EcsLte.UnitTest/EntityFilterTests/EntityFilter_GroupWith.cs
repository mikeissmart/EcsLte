using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityFilterTests
{
    [TestClass]
    public class EntityFilter_GroupWith : BasePrePostTest, IGroupWithTest
    {
        [TestMethod]
        public void GroupWith_SharedComponent()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            var filter = _context.FilterBy(Filter.AllOf<TestSharedComponent1>());
            var group = filter.GroupWith(new TestSharedComponent1 { Prop = 1 });

            // Correct group
            Assert.IsTrue(group != null);
            Assert.IsTrue(filter.GroupWith(component) == group);
            // Different component gets different entity
            Assert.IsTrue(filter.GroupWith(new TestSharedComponent1 { Prop = 2 }) != group);
            // Null component
            ISharedComponent nullKey = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                filter.GroupWith(nullKey));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.GroupWith(component));
        }

        [TestMethod]
        public void GroupWith_SharedComponents()
        {
            var component1 = new TestSharedComponent1 { Prop = 1 };
            var component2 = new TestSharedComponent2 { Prop = 1 };
            var filter = _context.FilterBy(Filter.AllOf<TestSharedComponent1>());
            var group = filter.GroupWith(component1, component2);

            // Correct group
            Assert.IsTrue(group != null);
            Assert.IsTrue(filter.GroupWith(component1, component2) == group);
            // Null component
            ISharedComponent nullComponent = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                filter.GroupWith(component1, component2, nullComponent));
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.GroupWith(component1, component2));
        }
    }
}