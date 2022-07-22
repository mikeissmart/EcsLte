using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityTrackerTests
{
    [TestClass]
    public class EntityTrackerTests : BasePrePostTest
    {
        [TestMethod]
        public void IsTrackingComponent()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added);

            Assert.IsTrue(tracker.IsTrackingComponent<TestComponent1>());
            Assert.IsFalse(tracker.IsTrackingComponent<TestComponent2>());

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.IsTrackingComponent<TestComponent1>());
        }

        [TestMethod]
        public void GetComponentState()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added);

            Assert.IsTrue(tracker.GetComponentState<TestComponent1>() == EntityTrackerState.Added);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.GetComponentState<TestComponent1>());
        }

        [TestMethod]
        public void SetComponentState()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added);

            Assert.IsTrue(tracker.GetComponentState<TestComponent1>() == EntityTrackerState.Added);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added));
        }

        [TestMethod]
        public void ClearComponentState()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added);

            Assert.IsTrue(tracker.IsTrackingComponent<TestComponent1>());

            tracker.ClearComponentState<TestComponent1>();
            Assert.IsFalse(tracker.IsTrackingComponent<TestComponent1>());

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.ClearComponentState<TestComponent1>());
        }

        [TestMethod]
        public void ClearAllComponentStates()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                .SetComponentState<TestComponent2>(EntityTrackerState.Added);

            tracker.ClearAllComponentStates();
            Assert.IsFalse(tracker.IsTrackingComponent<TestComponent1>());
            Assert.IsFalse(tracker.IsTrackingComponent<TestComponent2>());

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.ClearAllComponentStates());
        }

        [TestMethod]
        public void SetEntityStateChange()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetEntityStateChange(true);

            Assert.IsTrue(tracker.IsTrackingEntityStateChange);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.SetEntityStateChange(true));
        }

        [TestMethod]
        public void StartTracking()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .StartTracking();

            Assert.IsTrue(tracker.IsTracking);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.StartTracking());
        }

        [TestMethod]
        public void StopTracking()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .StartTracking()
                .StopTracking();

            Assert.IsFalse(tracker.IsTracking);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.StopTracking());
        }

        [TestMethod]
        public void ClearEntities()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetComponentState<TestComponent1>(EntityTrackerState.Added)
                .StartTracking();

            var entity = Context.Entities.CreateEntity(
                new EntityArcheType().AddComponentType<TestComponent1>(),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.HasEntity(entity, tracker));

            tracker.ClearEntities();
            Assert.IsFalse(Context.Entities.HasEntity(entity, tracker));

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.ClearEntities());
        }
    }
}