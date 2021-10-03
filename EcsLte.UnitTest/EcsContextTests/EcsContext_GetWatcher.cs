using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_GetWatcher : BasePrePostTest, IGetWatcherTest
    {
        [TestMethod]
        public void WatchAdded()
        {
            var watcher1 = _context.WatchAdded(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = _context.WatchAdded(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.WatchAdded(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void WatchUpdated()
        {
            var watcher1 = _context.WatchUpdated(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = _context.WatchUpdated(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.WatchUpdated(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void WatchRemoved()
        {
            var watcher1 = _context.WatchRemoved(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = _context.WatchRemoved(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.WatchRemoved(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void WatchAddedOrUpdated()
        {
            var watcher1 = _context.WatchAddedOrUpdated(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = _context.WatchAddedOrUpdated(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.WatchAddedOrUpdated(Filter.AllOf<TestComponent1>()));
        }

        [TestMethod]
        public void WatchAddedOrRemoved()
        {
            var watcher1 = _context.WatchAddedOrRemoved(Filter.AllOf<TestComponent1>());

            // Correct watcher
            Assert.IsTrue(watcher1 != null);
            // Same watcher
            var watcher2 = _context.WatchAddedOrRemoved(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(watcher1 == watcher2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.WatchAddedOrRemoved(Filter.AllOf<TestComponent1>()));
        }
    }
}