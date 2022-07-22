using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityDestroy : BasePrePostTest
    {
        [TestMethod]
        public void DestroyEntity()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>()
                    .AddSharedComponent(new TestSharedComponent1 { Prop = 2 })
                    .AddUniqueComponentType<TestUniqueComponent1>(),
                EntityState.Active);

            Context.Entities.DestroyEntity(entity);

            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsFalse(Context.Entities.HasEntity(entity));
            Assert.IsFalse(Context.Entities.HasUniqueComponent<TestUniqueComponent1>());

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
                    new EntityArcheType()
                        .AddComponentType<TestComponent1>(),
                    EntityState.Active, UnitTestConsts.SmallCount),
                () => new[] { Entity.Null, Entity.Null, Entity.Null, Entity.Null},
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
                (x, startingIndex, count) => Context.Entities.Context.Entities.DestroyEntities(x, startingIndex,count));

        }

        [TestMethod]
        public void DestroyEntities_ArcheType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = Context.Entities.CreateEntities(
                archeType,
                EntityState.Active, UnitTestConsts.SmallCount);

            Context.Entities.DestroyEntities(archeType);
            var result = AssertEntities(entities, 0, entities.Length);
            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            AssertArcheType_Invalid_Null(
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
            var filter = new EntityFilter()
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

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
            var tracker = Context.Tracking.CreateTracker("Tracker");
            tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

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
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var queryFilter = new EntityQuery(Context, filter);

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Context.Entities.DestroyEntities(queryFilter);
            var result = AssertEntities(entities, 0, entities.Length);
            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            var queryFilterTracker = new EntityQuery(
                Context.Tracking.CreateTracker("Tracker"),
                filter);
            queryFilterTracker.Tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            queryFilterTracker.Tracker.StartTracking();

            entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            Context.Entities.DestroyEntities(queryFilterTracker);
            result = AssertEntities(entities, 0, entities.Length);
            Assert.IsTrue(Context.Entities.EntityCount() == 0);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                new EntityQuery(EcsContexts.CreateContext("DiffContext"), filter),
                queryFilterTracker,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.DestroyEntities(x)
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.DestroyEntities(queryFilter));
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
