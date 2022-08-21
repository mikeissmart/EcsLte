using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityTrackerTests
{
    [TestClass]
    public class EntityTrackerTests : BasePrePostTest
    {
        [TestMethod]
        public void GetTrackingState()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingState<TestComponent1>(TrackingState.Added);

            Assert.IsTrue(tracker.GetTrackingState<TestComponent1>() == TrackingState.Added);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.GetTrackingState<TestComponent1>());
        }

        [TestMethod]
        public void SetTrackingState()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingState<TestComponent1>(TrackingState.Added);

            Assert.IsTrue(tracker.GetTrackingState<TestComponent1>() == TrackingState.Added);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.SetTrackingState<TestComponent1>(TrackingState.Added));
        }

        [TestMethod]
        public void ClearAllTrackingStates()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingState<TestComponent1>(TrackingState.Added)
                .SetTrackingState<TestComponent2>(TrackingState.Added);

            tracker.ClearAllTrackingStates();
            Assert.IsFalse(tracker.GetTrackingState<TestComponent1>() == TrackingState.Added);
            Assert.IsFalse(tracker.GetTrackingState<TestComponent2>() == TrackingState.Added);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.ClearAllTrackingStates());
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
                .SetTrackingState<TestComponent1>(TrackingState.Added)
                .StartTracking();

            var entity = Context.Entities.CreateEntity(
                Context.ArcheTypes.AddComponentType<TestComponent1>());

            Assert.IsTrue(Context.Entities.HasEntity(entity, tracker));

            tracker.ClearEntities();
            Assert.IsFalse(Context.Entities.HasEntity(entity, tracker));

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.ClearEntities());
        }
    }
}