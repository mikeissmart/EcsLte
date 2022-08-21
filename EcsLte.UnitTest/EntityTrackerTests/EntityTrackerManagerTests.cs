using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityTrackerTests
{
    [TestClass]
    public class EntityTrackerManagerTests : BasePrePostTest
    {
        [TestMethod]
        public void HasTracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");

            Assert.IsTrue(Context.Tracking.HasTracker("Tracker"));
            Assert.IsFalse(Context.Tracking.HasTracker("Tracker1"));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Tracking.HasTracker(null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Tracking.HasTracker("Tracking"));
        }

        [TestMethod]
        public void GetTracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");

            Assert.IsTrue(Context.Tracking.GetTracker("Tracker") == tracker);

            Assert.ThrowsException<EntityTrackerNotExistException>(() =>
                Context.Tracking.GetTracker("Tracker Not Exist"));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Tracking.GetTracker(null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Tracking.GetTracker("Tracking"));
        }

        [TestMethod]
        public void CreateTracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");

            Assert.IsTrue(tracker != null);
            Assert.IsTrue(tracker.Context == Context);
            Assert.IsTrue(tracker.Name == "Tracker");
            Assert.IsTrue(tracker.IsTracking == false);
            Assert.IsTrue(tracker.IsDestroyed == false);

            Assert.ThrowsException<EntityTrackerAlreadyExistException>(() =>
                Context.Tracking.CreateTracker("Tracker"));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Tracking.CreateTracker(null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Tracking.CreateTracker("Tracker"));
        }

        [TestMethod]
        public void RemoveTracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");
            Context.Tracking.RemoveTracker(tracker);

            Assert.IsFalse(Context.Tracking.HasTracker(tracker.Name));
            Assert.IsTrue(tracker.IsDestroyed);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                Context.Tracking.RemoveTracker(tracker));

            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Tracking.RemoveTracker(null));

            var tracker2 = EcsContexts.CreateContext("Context2")
                .Tracking.CreateTracker("Tracker");
            Assert.ThrowsException<EcsContextNotSameException>(() =>
                Context.Tracking.RemoveTracker(tracker2));

            tracker2 = Context.Tracking.CreateTracker("Tracker2");

            EcsContexts.DestroyContext(Context);

            Assert.IsTrue(tracker2.IsDestroyed);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Tracking.RemoveTracker(tracker2));
        }
    }
}
