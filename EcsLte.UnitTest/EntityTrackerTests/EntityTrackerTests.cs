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
            var tracker = Context.Tracking
                .SetTrackingComponent<TestComponent1>(true);

            Assert.IsTrue(tracker.IsTrackingComponent<TestComponent1>());
        }

        [TestMethod]
        public void SetTrackingState()
        {
            var tracker = Context.Tracking
                .SetTrackingComponent<TestComponent1>(true);

            Assert.IsTrue(tracker.IsTrackingComponent<TestComponent1>());
        }

        [TestMethod]
        public void SetAllTrackingComponents()
        {
            var tracker = Context.Tracking
                .SetTrackingComponent<TestComponent1>(true)
                .SetTrackingComponent<TestComponent2>(true);

            tracker.SetAllTrackingComponents(false);
            Assert.IsFalse(tracker.IsTrackingComponent<TestComponent1>());
            Assert.IsFalse(tracker.IsTrackingComponent<TestComponent2>());
        }
    }
}