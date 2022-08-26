using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTests : BasePrePostTest
    {
        [TestMethod]
        public void SetCommands()
        {
            var commands = Context.Commands
                .CreateCommands("Test");
            var query = Context.Queries
                .SetCommands(commands);

            Assert.IsTrue(query.Commands == commands);

            Context.Commands.RemoveCommands(commands);
            Assert.ThrowsException<EntityCommandsIsDestroyedException>(() =>
                query.SetCommands(commands));

            var diffCommands = EcsContexts.CreateContext("Diff").Commands
                .CreateCommands("DiffTest");
            Assert.ThrowsException<EcsContextNotSameException>(() =>
                query.SetCommands(diffCommands));
        }

        [TestMethod]
        public void ClearCommands()
        {
            var commands = Context.Commands
                .CreateCommands("Test");
            var query = Context.Queries
                .SetCommands(commands)
                .ClearCommands();

            Assert.IsTrue(query.Commands == null);
        }

        [TestMethod]
        public void SetFilters()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();
            var query = Context.Queries
                .SetFilter(filter);

            Assert.IsTrue(query.Filter == filter);

            var diffFilters = EcsContexts.CreateContext("Diff").Filters
                .WhereAllOf<TestComponent1>();
            Assert.ThrowsException<EcsContextNotSameException>(() =>
                query.SetFilter(diffFilters));
        }

        [TestMethod]
        public void ClearFilters()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();
            var query = Context.Queries
                .SetFilter(filter)
                .ClearFilter();

            Assert.IsTrue(query.Filter == null);
        }

        [TestMethod]
        public void SetTracker()
        {
            var tracker = Context.Tracking
                .CreateTracker("Test");
            var query = Context.Queries
                .SetTracker(tracker);

            Assert.IsTrue(query.Tracker == tracker);

            Context.Tracking.RemoveTracker(tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                query.SetTracker(tracker));

            var diffTracking = EcsContexts.CreateContext("Diff").Tracking
                .CreateTracker("DiffTest");
            Assert.ThrowsException<EcsContextNotSameException>(() =>
                query.SetTracker(diffTracking));
        }

        [TestMethod]
        public void ClearTracking()
        {
            var tracker = Context.Tracking
                .CreateTracker("Test");
            var query = Context.Queries
                .SetTracker(tracker)
                .ClearTracker();

            Assert.IsTrue(query.Tracker == null);
        }

        [TestMethod]
        public void Run()
        {
            var query = Context.Queries
                .SetFilter(Context.Filters.WhereAllOf<TestComponent1>());

            Assert.ThrowsException<EntityQueryHasNoForEachException>(() =>
                query.Run());

            query.ForEach((int threadIndex, int index, Entity entity) => { });
            query.Run();
        }
    }
}