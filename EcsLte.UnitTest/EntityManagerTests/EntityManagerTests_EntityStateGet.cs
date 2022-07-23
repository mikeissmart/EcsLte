using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityStateGet : BasePrePostTest
    {
        [TestMethod]
        public void GetEntityState()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active);

            Assert.IsTrue(Context.Entities.GetEntityState(entity) == EntityState.Active);

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetEntityState(Entity.Null));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetEntityState(entity));
        }

        [TestMethod]
        public void GetEntitysStates()
        {
            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            var getStates = Context.Entities.GetEntityStates(entities);
            var getResult = AssertStates(EntityState.Active, getStates,
                0, entities.Length);
            Assert.IsTrue(getStates.Length == entities.Length);
            Assert.IsTrue(getResult.Success, $"Valid: {getResult.Error}");

            AssertGetInRef_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => entities,
                () => new[] { Entity.Null, Entity.Null, Entity.Null, Entity.Null },
                x => Context.Entities.GetEntityStates(x),
                (x, startingIndex) => Context.Entities.GetEntityStates(x, startingIndex),
                (x, startingIndex, count) => Context.Entities.GetEntityStates(x, startingIndex, count),
                (inSrc, x) =>
                {
                    var count = Context.Entities.GetEntityStates(inSrc, ref x);
                    return (x, count);
                },
                (inSrc, startingIndex, x) =>
                {
                    var count = Context.Entities.GetEntityStates(inSrc, startingIndex, ref x);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    Context.Entities.GetEntityStates(inSrc, startingIndex, count, ref x);
                    return x;
                },
                (inSrc, x, destStartingIndex) =>
                {
                    var count = Context.Entities.GetEntityStates(inSrc, ref x, destStartingIndex);
                    return (x, count);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    var count = Context.Entities.GetEntityStates(inSrc, startingIndex, ref x, destStartingIndex);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    Context.Entities.GetEntityStates(inSrc, startingIndex, count, ref x, destStartingIndex);
                    return x;
                },
                (inSrc, startingIndex, outRef, destStartingIndex, destCount) =>
                {
                    var result = new TestResult();
                    if (outRef.Length - destStartingIndex != destCount)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {outRef.Length}, Dest Count: {destCount}";
                    }
                    else
                        result = AssertStates(EntityState.Active, outRef, destStartingIndex, destCount);
                    return result;
                });

            EcsContexts.DestroyContext(Context);
            AssertGetInRef_ContextDestroyed<Entity, EntityState>(
                x => Context.Entities.GetEntityStates(x),
                (x, startingIndex) => Context.Entities.GetEntityStates(x, startingIndex),
                (x, startingIndex, count) => Context.Entities.GetEntityStates(x, startingIndex, count),
                (inSrc, x) => Context.Entities.GetEntityStates(inSrc, ref x),
                (inSrc, startingIndex, x) => Context.Entities.GetEntityStates(inSrc, startingIndex, ref x),
                (inSrc, startingIndex, count, x) => Context.Entities.GetEntityStates(inSrc, startingIndex, count, ref x),
                (inSrc, x, destStartingIndex) => Context.Entities.GetEntityStates(inSrc, ref x, destStartingIndex),
                (inSrc, startingIndex, x, destStartingIndex) => Context.Entities.GetEntityStates(inSrc, startingIndex, ref x, destStartingIndex),
                (inSrc, startingIndex, count, x, destStartingIndex) => Context.Entities.GetEntityStates(inSrc, startingIndex, count, ref x, destStartingIndex));
        }

        [TestMethod]
        public void GetEntitysStates_ArcheType()
        {
            var archeType = new EntityArcheType()
                    .AddComponentType<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            var getStates = Context.Entities.GetEntityStates(archeType);
            var getResult = AssertStates(EntityState.Active, getStates, 0, entities.Length);
            Assert.IsTrue(getStates.Length == entities.Length);
            Assert.IsTrue(getResult.Success, $"Valid: {getResult.Error}");

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntityStates(archeType),
                x =>
                {
                    var count = Context.Entities.GetEntityStates(archeType, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntityStates(archeType, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertStates(EntityState.Active, x, startingIndex, count);
                    return result;
                });

            var emptyStates = new EntityState[0];
            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x => Context.Entities.GetEntityStates(x),
                    x => Context.Entities.GetEntityStates(x, ref emptyStates),
                    x => Context.Entities.GetEntityStates(x, ref emptyStates, 0),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetEntityStates(archeType));
            AssertGetRef_ContextDestroyed<EntityState>(
                () => Context.Entities.GetEntityStates(archeType),
                x => Context.Entities.GetEntityStates(archeType, ref x),
                (x, startingIndex) => Context.Entities.GetEntityStates(archeType, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntitysStates_Filter()
        {
            var filter = new EntityFilter()
                    .WhereAllOf<TestComponent1>();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            var getStates = Context.Entities.GetEntityStates(filter);
            var getResult = AssertStates(EntityState.Active, getStates, 0, entities.Length);
            Assert.IsTrue(getStates.Length == entities.Length);
            Assert.IsTrue(getResult.Success, $"Valid: {getResult.Error}");

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntityStates(filter),
                x =>
                {
                    var count = Context.Entities.GetEntityStates(filter, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntityStates(filter, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertStates(EntityState.Active, x, startingIndex, count);
                    return result;
                });

            var emptyStates = new EntityState[0];
            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x => Context.Entities.GetEntityStates(x),
                    x => Context.Entities.GetEntityStates(x, ref emptyStates),
                    x => Context.Entities.GetEntityStates(x, ref emptyStates, 0),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetEntityStates(filter));
            AssertGetRef_ContextDestroyed<EntityState>(
                () => Context.Entities.GetEntityStates(filter),
                x => Context.Entities.GetEntityStates(filter, ref x),
                (x, startingIndex) => Context.Entities.GetEntityStates(filter, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntitysStates_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");
            tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            var getStates = Context.Entities.GetEntityStates(tracker);
            var getResult = AssertStates(EntityState.Active, getStates, 0, entities.Length);
            Assert.IsTrue(getStates.Length == entities.Length);
            Assert.IsTrue(getResult.Success, $"Valid: {getResult.Error}");

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntityStates(tracker),
                x =>
                {
                    var count = Context.Entities.GetEntityStates(tracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntityStates(tracker, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertStates(EntityState.Active, x, startingIndex, count);
                    return result;
                });

            var diffContext = EcsContexts.CreateContext("DiffContext")
                .Tracking.CreateTracker("Tracker");
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            var refStates = new EntityState[0];
            AssertTracker_Destroyed_Null(
                diffContext,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x => Context.Entities.GetEntityStates(x),
                    x => Context.Entities.GetEntityStates(x, ref refStates),
                    x => Context.Entities.GetEntityStates(x, ref refStates, 0),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetEntityStates(tracker));
            AssertGetRef_ContextDestroyed<EntityState>(
                () => Context.Entities.GetEntityStates(tracker),
                x => Context.Entities.GetEntityStates(tracker, ref x),
                (x, startingIndex) => Context.Entities.GetEntityStates(tracker, ref x, startingIndex));
        }

        [TestMethod]
        public void GetEntitysStates_Query()
        {
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var queryFilter = new EntityQuery(Context, filter);

            var queryFilterTracker = new EntityQuery(Context.Tracking.CreateTracker("Tracker1"),
                filter);
            queryFilterTracker.Tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            queryFilterTracker.Tracker.StartTracking();

            var entities = Context.Entities.CreateEntities(
                new EntityArcheType()
                    .AddComponentType<TestComponent1>(),
                EntityState.Active, UnitTestConsts.SmallCount);

            var getStates = Context.Entities.GetEntityStates(queryFilter);
            var getResult = AssertStates(EntityState.Active, getStates, 0, entities.Length);
            Assert.IsTrue(getStates.Length == entities.Length);
            Assert.IsTrue(getResult.Success, $"Valid: {getResult.Error}");

            getStates = Context.Entities.GetEntityStates(queryFilterTracker);
            getResult = AssertStates(EntityState.Active, getStates, 0, entities.Length);
            Assert.IsTrue(getStates.Length == entities.Length);
            Assert.IsTrue(getResult.Success, $"Valid: {getResult.Error}");

            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntityStates(queryFilter),
                x =>
                {
                    var count = Context.Entities.GetEntityStates(queryFilter, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntityStates(queryFilter, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertStates(EntityState.Active, x, startingIndex, count);
                    return result;
                });
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () => Context.Entities.GetEntityStates(queryFilterTracker),
                x =>
                {
                    var count = Context.Entities.GetEntityStates(queryFilterTracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.GetEntityStates(queryFilterTracker, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {entities.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                        result = AssertStates(EntityState.Active, x, startingIndex, count);
                    return result;
                });

            Context.Tracking.RemoveTracker(queryFilterTracker.Tracker);
            var refStates = new EntityState[0];
            AssertQuery_DiffContext_DestroyedTracker_Null(
                new EntityQuery(EcsContexts.CreateContext("DiffContext"), filter),
                queryFilterTracker,
                new Action<EntityQuery>[]
                {
                    x => Context.Entities.GetEntityStates(x),
                    x => Context.Entities.GetEntityStates(x, ref refStates),
                    x => Context.Entities.GetEntityStates(x, ref refStates, 0),
                });

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetEntityStates(queryFilter));
            AssertGetRef_ContextDestroyed<EntityState>(
                () => Context.Entities.GetEntityStates(queryFilter),
                x => Context.Entities.GetEntityStates(queryFilter, ref x),
                (x, startingIndex) => Context.Entities.GetEntityStates(queryFilter, ref x, startingIndex));
        }

        private TestResult AssertStates(EntityState state, EntityState[] states,
            int startingIndex, int count)
        {
            var result = new TestResult();
            for (var i = startingIndex; i < count; i++)
            {
                if (states[i] != state)
                {
                    result.Success = false;
                    result.Error = $"EntityState: {states[i]}, StartingIndex: {startingIndex}";
                    break;
                }
            }
            return result;
        }
    }
}
