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
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount() == entities.Length);

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount());
        }

        [TestMethod]
        public void EntityCount_ArcheType()
        {
            var archetype = new EntityArcheType()
                    .AddComponentType<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(archetype) == entities.Length);

            AssertArcheType_Invalid_Null(
               new Action<EntityArcheType>[]
               {
                   x => Context.Entities.EntityCount(x)
               });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount());
        }

        [TestMethod]
        public void EntityCount_Filter()
        {
            var filter = new EntityFilter()
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(filter) == entities.Length);

            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.EntityCount(x)
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount(filter));
        }

        [TestMethod]
        public void EntityCount_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker1");
            tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(tracker) == entities.Length);

            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            AssertTracker_Destroyed_Null(
                EcsContexts.CreateContext("DiffContext")
                    .Tracking.CreateTracker("Tracker"),
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.EntityCount(x)
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount(tracker));
        }

        [TestMethod]
        public void EntityCount_Query()
        {
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var queryFilter = new EntityQuery(Context, filter);

            var queryFilterTracker = new EntityQuery(
                Context.Tracking.CreateTracker("Tracker"),
                filter);
            queryFilterTracker.Tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            queryFilterTracker.Tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(queryFilter) == entities.Length);
            Assert.IsTrue(Context.Entities.EntityCount(queryFilterTracker) == entities.Length);

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                new EntityQuery(EcsContexts.CreateContext("DiffContext"), filter),
                queryFilterTracker,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.EntityCount(x)
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount(queryFilter));
        }
    }
}
