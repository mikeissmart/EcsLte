using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityFilterTests
{
    [TestClass]
    public class EntityFilter_GetWatcher : BasePrePostTest, IGetWatcherTest
    {
        [TestMethod]
        public void Added()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.Added(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.Added(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.Added(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void Updated()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.Updated(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.Updated(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.Updated(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void Removed()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.Removed(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.Removed(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.Removed(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void AddedOrUpdated()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.AddedOrUpdated(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.AddedOrUpdated(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.AddedOrUpdated(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void AddedOrRemoved()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.AddedOrRemoved(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.AddedOrRemoved(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.AddedOrRemoved(Filter.AllOf<TestComponent1>()));
        }
    }
}