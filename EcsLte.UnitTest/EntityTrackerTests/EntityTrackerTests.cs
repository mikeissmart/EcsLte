using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityTrackerTests
{
    [TestClass]
    public class EntityTrackerTests : BasePrePostTest
    {
        [TestMethod]
        public void IsTrackingComponent()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingComponent<TestComponent1>(true);

            Assert.IsTrue(tracker.IsTrackingComponent<TestComponent1>());

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.IsTrackingComponent<TestComponent1>());
        }

        [TestMethod]
        public void SetTrackingState()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingComponent<TestComponent1>(true);

            Assert.IsTrue(tracker.IsTrackingComponent<TestComponent1>());

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.SetTrackingComponent<TestComponent1>(true));
        }

        [TestMethod]
        public void SetAllTrackingComponents()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingComponent<TestComponent1>(true)
                .SetTrackingComponent<TestComponent2>(true);

            tracker.SetAllTrackingComponents(false);
            Assert.IsFalse(tracker.IsTrackingComponent<TestComponent1>());
            Assert.IsFalse(tracker.IsTrackingComponent<TestComponent2>());

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.SetAllTrackingComponents(false));
        }

        // todo remove
        /*[TestMethod]
        public void StartTracking()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .StartTracking();

            Assert.IsTrue(tracker.IsTracking);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.StartTracking());
        }*/

        // todo remove
        /*[TestMethod]
        public void StopTracking()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .StartTracking()
                .StopTracking();

            Assert.IsFalse(tracker.IsTracking);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.StopTracking());
        }*/

        // todo remove
        /*[TestMethod]
        public void ClearEntities()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingComponent<TestComponent1>(true);

            var entity = Context.Entities.CreateEntity(
                Context.ArcheTypes.AddComponentType<TestComponent1>());

            Assert.IsTrue(Context.Entities.HasEntity(entity, tracker));

            tracker.ClearEntities();
            Assert.IsFalse(Context.Entities.HasEntity(entity, tracker));

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                tracker.ClearEntities());
        }*/
    }
}