using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityFilterTests
{
    [TestClass]
    public class EntityFilter_GetWatcher : BasePrePostTest, IGetWatcherTest
    {
        [TestMethod]
        public void WatchAdded()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.WatchAdded(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.WatchAdded(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.WatchAdded(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void WatchUpdated()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.WatchUpdated(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.WatchUpdated(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.WatchUpdated(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void WatchRemoved()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.WatchRemoved(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.WatchRemoved(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.WatchRemoved(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void WatchAddedOrUpdated()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.WatchAddedOrUpdated(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.WatchAddedOrUpdated(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.WatchAddedOrUpdated(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void WatchAddedOrRemoved()
        {
            var filter = _context.FilterBy(Filter.AllOf<TestComponent1>());
            var watcher1 = filter.WatchAddedOrRemoved(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = filter.WatchAddedOrRemoved(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                filter.WatchAddedOrRemoved(Filter.AllOf<TestComponent1>()));
        }
    }
}