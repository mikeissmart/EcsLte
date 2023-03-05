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
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>());

            Assert.IsTrue(Context.Entities.HasEntity(entity));
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasEntity(entity));
        }

        [TestMethod]
        public void HasEntity_ArcheType()
        {
            var archeType = Context.ArcheTypes
                    .AddComponentType<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(archeType) == entities.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.HasEntity(entities[i], archeType),
                    $"Valid: {entities[i]}");
            }
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null, archeType));

            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.HasEntity(entities[0], x),
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasEntity(entities[0], archeType));
        }

        [TestMethod]
        public void HasEntity_Filter()
        {
            var filter = Context.Filters
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

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

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasEntity(entities[0], filter));
        }

        [TestMethod]
        public void HasEntity_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker1")
                .SetTrackingComponent<TestComponent1>(true);

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

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
                EcsContexts.Instance.CreateContext("DiffContext")
                    .Tracking.CreateTracker("Tracker"),
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.HasEntity(entities[0], x),
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.HasEntity(entities[0], tracker));
        }

        [TestMethod]
        public void HasEntity_Query()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            var query = Context.Queries
                .SetFilter(filter)
                .SetTracker(Context.Tracking.CreateTracker("Tracker1")
                    .SetTrackingComponent<TestComponent1>(true));

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Assert.IsTrue(Context.Entities.EntityCount(query) == entities.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.HasEntity(entities[i], query),
                    $"Valid Filter: {entities[i]}");
            }
            Assert.IsFalse(Context.Entities.HasEntity(Entity.Null, query));

            Context.Tracking.RemoveTracker(query.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                query,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.HasEntity(entities[0], x),
                });

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.EntityCount(query));
        }
    }
}
