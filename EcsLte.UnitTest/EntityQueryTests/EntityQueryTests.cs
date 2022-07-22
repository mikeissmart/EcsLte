using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTests : BasePrePostTest
    {
        [TestMethod]
        public void Create_Filter()
        {
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();
            var query = new EntityQuery(Context, filter);

            Assert.IsTrue(query.Context == Context);
            Assert.IsTrue(query.Filter == filter);
            Assert.IsTrue(query.Tracker == null);
        }

        [TestMethod]
        public void Create_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();
            var query = new EntityQuery(tracker, filter);

            Assert.IsTrue(query.Context == Context);
            Assert.IsTrue(query.Filter == filter);
            Assert.IsTrue(query.Tracker == tracker);
        }
    }
}