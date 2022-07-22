using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityHas : BasePrePostTest
    {
        [TestMethod]
        public void HasEntity()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.HasEntity(entity));
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasEntity(entity));
        }

        [TestMethod]
        public void HasEntity_ArcheType()
        {
            var archeType = new EntityArcheType()
                    .AddComponentType<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(archeType) == entities.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.HasEntity(entities[i], archeType),
                    $"Valid: {entities[i]}");
            }
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null, archeType));

            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.HasEntity(entities[0], x),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasEntity(entities[0], archeType));
        }

        [TestMethod]
        public void HasEntity_Filter()
        {
            var filter = new EntityFilter()
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(filter) == entities.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.HasEntity(entities[i], filter),
                    $"Valid: {entities[i]}");
            }
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null, filter));

            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.HasEntity(entities[0], x),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasEntity(entities[0], filter));
        }

        [TestMethod]
        public void HasEntity_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker1");
            tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(tracker) == entities.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.HasEntity(entities[i], tracker),
                    $"Valid: {entities[i]}");
            }
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null, tracker));

            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            AssertTracker_Destroyed_Null(
                EcsContexts.CreateContext("DiffContext")
                    .Tracking.CreateTracker("Tracker"),
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.HasEntity(entities[0], x),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasEntity(entities[0], tracker));
        }

        [TestMethod]
        public void HasEntity_Query()
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
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.HasEntity(entities[i], queryFilter),
                    $"Valid Filter: {entities[i]}");
                Assert.IsTrue(Context.Entities.HasEntity(entities[i], queryFilterTracker),
                    $"Valid Filter Tracker: {entities[i]}");
            }
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null, queryFilter));
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null, queryFilterTracker));

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                new EntityQuery(EcsContexts.CreateContext("DiffContext"), filter),
                queryFilterTracker,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.HasEntity(entities[0], x),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount(queryFilter));
        }
    }
}
