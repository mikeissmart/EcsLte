using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityDestroy : BasePrePostTest
    {
        [TestMethod]
        public void DestroyEntity()
        {
            var entity = Context.Entities.CreateEntity(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 })
                    .AddManagedComponentType<TestManagedComponent1>());

            Context.Entities.DestroyEntity(entity);

            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsFalse(Context.Entities.HasEntity(entity));

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.DestroyEntity(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.DestroyEntity(entity));
        }

        [TestMethod]
        public void DestroyEntities()
        {
            AssertGetIn_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.CreateEntities(
                    Context.ArcheTypes
                        .AddComponentType<TestComponent1>(),
                    UnitTestConsts.SmallCount),
                () => new[] { Entity.Null, Entity.Null, Entity.Null, Entity.Null },
                x =>
                {
                    Context.Entities.DestroyEntities(x);
                    return new int[x.Length];
                },
                (x, offset) =>
                {
                    Context.Entities.DestroyEntities(x, offset);
                    return new int[x.Length - offset];
                },
                (x, offset, count) =>
                {
                    Context.Entities.DestroyEntities(x, offset, count);
                    return new int[count];
                },
                (src, offset, count, destroyed) =>
                {
                    var result = new TestResult();
                    if (Context.Entities.EntityCount() != src.Length - destroyed.Length)
                    {
                        result.Success = false;
                        result.Error = $"EntityCount: {Context.Entities.EntityCount()}";
                    }
                    else
                    {
                        result = AssertEntities(src, offset, count);
                        if (result.Success && Context.Entities.EntityCount() > 0)
                        {
                            // Clear rest of entities
                            Context.Entities.DestroyEntities(Context.Entities.GetEntities());
                        }
                    }
                    return result;
                });

            var entities = new Entity[0];
            EcsContexts.DestroyContext(Context);
            AssertGetIn_ContextDestroyed<Entity>(
                x => Context.Entities.Context.Entities.DestroyEntities(x),
                (x, startingIndex) => Context.Entities.Context.Entities.DestroyEntities(x, startingIndex),
                (x, startingIndex, count) => Context.Entities.Context.Entities.DestroyEntities(x, startingIndex, count));

        }

        [TestMethod]
        public void DestroyEntities_ArcheType()
        {
            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = Context.Entities.CreateEntities(
                archeType,
                UnitTestConsts.SmallCount);

            Context.Entities.DestroyEntities(archeType);
            var result = AssertEntities(entities, 0, entities.Length);
            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            AssertArcheType_DiffContext_Null(
               new Action<EntityArcheType>[]
               {
                   x => Context.Entities.DestroyEntities(x)
               });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.DestroyEntities(archeType));
        }

        [TestMethod]
        public void DestroyEntities_Filter()
        {
            var filter = Context.Filters
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Context.Entities.DestroyEntities(filter);
            var result = AssertEntities(entities, 0, entities.Length);
            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.DestroyEntities(x)
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.DestroyEntities(filter));
        }

        [TestMethod]
        public void DestroyEntities_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker1")
                .SetTrackingState<TestComponent1>(TrackingState.Added)
                .StartTracking();

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Context.Entities.DestroyEntities(tracker);
            var result = AssertEntities(entities, 0, entities.Length);
            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            var diffContext = EcsContexts.CreateContext("DiffContext")
                .Tracking.CreateTracker("Tracker");
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            AssertTracker_Destroyed_Null(
                diffContext,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.DestroyEntities(x)
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.DestroyEntities(tracker));
        }

        [TestMethod]
        public void DestroyEntities_Query()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            var query = Context.Queries
                .SetFilter(filter)
                .SetTracker(Context.Tracking.CreateTracker("Tracker1")
                    .SetTrackingState<TestComponent1>(TrackingState.Added)
                    .StartTracking());

            var entities = Context.Entities.CreateEntities(
                Context.ArcheTypes
                    .AddComponentType<TestComponent1>(),
                UnitTestConsts.SmallCount);

            Context.Entities.DestroyEntities(query);
            var result = AssertEntities(entities, 0, entities.Length);
            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            Context.Tracking.RemoveTracker(query.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                query,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.DestroyEntities(x)
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.DestroyEntities(query));
        }

        private TestResult AssertEntities(Entity[] entities, int startingIndex, int count)
        {
            var result = new TestResult();
            for (var i = 0; i < count; i++)
            {
                if (Context.Entities.HasEntity(entities[i + startingIndex]))
                {
                    result.Success = false;
                    result.Error = $"Entity: {entities[i]}";
                    break;
                }
            }
            return result;
        }
    }
}
