using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityCount : BasePrePostTest
    {
        [TestMethod]
        public void EntityCount()
        {
            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount() == entities.Length);

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount());
        }

        [TestMethod]
        public void EntityCount_ArcheType()
        {
            var archetype = Context.ArcheTypes
                    .AddComponentType<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(archetype) == entities.Length);

            AssertArcheType_DiffContext_Null(
               new Action<EntityArcheType>[]
               {
                   x => Context.Entities.EntityCount(x)
               });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount());
        }

        [TestMethod]
        public void EntityCount_Filter()
        {
            var filter = Context.Filters
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(filter) == entities.Length);

            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.EntityCount(x)
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount(filter));
        }

        [TestMethod]
        public void EntityCount_Tracker()
        {
            var tracker = Context.Tracking
                .SetTrackingComponent<TestComponent1>(true);

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(tracker) == entities.Length);

            AssertTracker_Different_Null(
                EcsContexts.Instance.CreateContext("DiffContext")
                    .Tracking
                .SetTrackingMode(EntityTrackerMode.AnyChanges),
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.EntityCount(x)
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount(tracker));
        }

        [TestMethod]
        public void EntityCount_Query()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            var query = Context.Queries
                .SetFilter(filter)
                .SetTracker(Context.Tracking
                    .SetTrackingComponent<TestComponent1>(true));

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(query) == entities.Length);

            AssertQuery_DiffContext_Null(
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.EntityCount(x)
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount(query));
        }
    }
}
