using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityStateSet : BasePrePostTest
    {
        [TestMethod]
        public void SetEntityState()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active);

            Context.Entities.SetEntityState(entity, EntityState.Destroying);

            Assert.IsTrue(Context.Entities.GetEntityState(entity) == EntityState.Destroying);

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.SetEntityState(Entity.Null, EntityState.Destroying));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.SetEntityState(entity, EntityState.Destroying));
        }

        [TestMethod]
        public void SetEntityStates()
        {
            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Creating, UnitTestConsts.SmallCount);

            var state = EntityState.Creating;
            AssertGetIn_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => entities,
                () => new[] { Entity.Null, Entity.Null, Entity.Null, Entity.Null },
                x =>
                {
                    state = EntityState.Active;
                    Context.Entities.SetEntityStates(x, state);
                    return new int[0];
                },
                (x, startingIndex) =>
                {
                    state = EntityState.InActive;
                    Context.Entities.SetEntityStates(x, startingIndex, state);
                    return new int[0];
                },
                (x, startingIndex, count) =>
                {
                    state = EntityState.Destroying;
                    Context.Entities.SetEntityStates(x, startingIndex, count, state);
                    return new int[0];
                },
                (src, startingIndex, count, srcOut) =>
                {
                    return AssertStates(src, state, startingIndex, count);
                });

            EcsContexts.DestroyContext(Context);
            AssertGetIn_ContextDestroyed<Entity>(
                x => Context.Entities.Context.Entities.SetEntityStates(x, EntityState.Active),
                (x, startingIndex) => Context.Entities.Context.Entities.SetEntityStates(x, startingIndex, EntityState.Active),
                (x, startingIndex, count) => Context.Entities.Context.Entities.SetEntityStates(x, startingIndex, count, EntityState.Active));
        }

        [TestMethod]
        public void SetEntityStates_ArcheType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Creating, UnitTestConsts.SmallCount);

            Context.Entities.SetEntityStates(archeType, EntityState.Active);
            var result = AssertStates(entities, EntityState.Active, 0, entities.Length);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            AssertArcheType_Invalid_Null(
               new Action<EntityArcheType>[]
               {
                   x => Context.Entities.SetEntityStates(x, EntityState.Active)
               });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.SetEntityStates(archeType, EntityState.Active));
        }

        [TestMethod]
        public void SetEntityStates_Filter()
        {
            var filter = new EntityFilter()
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Creating, UnitTestConsts.SmallCount);

            Context.Entities.SetEntityStates(filter, EntityState.Active);
            var result = AssertStates(entities, EntityState.Active, 0, entities.Length);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            AssertFilter_Null(
                new Action<EntityFilter>[]
               {
                   x => Context.Entities.SetEntityStates(x, EntityState.Active)
               });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.SetEntityStates(filter, EntityState.Active));
        }

        [TestMethod]
        public void SetEntityStates_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");
            tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Creating, UnitTestConsts.SmallCount);

            Context.Entities.SetEntityStates(tracker, EntityState.Active);
            var result = AssertStates(entities, EntityState.Active, 0, entities.Length);
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
                    x => Context.Entities.SetEntityStates(x, EntityState.Active)
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.SetEntityStates(tracker, EntityState.Active));
        }

        [TestMethod]
        public void SetEntityStates_Query()
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
                EntityState.Creating, UnitTestConsts.SmallCount);

            Context.Entities.SetEntityStates(queryFilter, EntityState.Active);
            var result = AssertStates(entities, EntityState.Active, 0, entities.Length);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            Context.Entities.SetEntityStates(queryFilterTracker, EntityState.InActive);
            result = AssertStates(entities, EntityState.InActive, 0, entities.Length);
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
                Context.Entities.SetEntityStates(queryFilter, EntityState.Active));
        }

        private TestResult AssertStates(Entity[] entities, EntityState state,
            int startingIndex, int count)
        {
            var result = new TestResult();
            for (var i = startingIndex; i < count; i++)
            {
                if (Context.Entities.GetEntityState(entities[i]) != state)
                {
                    result.Success = false;
                    result.Error = $"Entity: {entities[i]}, EntityState: {state}, StartingIndex: {startingIndex}";
                    break;
                }
            }
            return result;
        }
    }
}
